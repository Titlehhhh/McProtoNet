using System.Buffers;
using System.Runtime.ExceptionServices;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
///     Streaming writer: framing and encryption happen synchronously into one pooled buffer, and
///     <see cref="FlushAsync" /> hands the whole buffer to the stream in a single write. When the
///     flush returns, the bytes are at the socket.
/// </summary>
/// <remarks>
///     One owner: there is no lock inside. A flush that fails or is cancelled kills the writer for
///     good — every member throws that failure afterwards, because part of a frame may already be
///     on the wire.
/// </remarks>
internal sealed class BufferedPacketWriter : IDisposable
{
    private const int DefaultCapacity = 64 * 1024;

    private readonly Stream _stream;
    private readonly PacketCipher? _cipher;
    private readonly int _compressionThreshold;

    private PooledBufferWriter _buffer;
    private Exception? _fault;
    private int _flushing;
    private bool _disposed;

    public BufferedPacketWriter(Stream stream, int compressionThreshold = -1, PacketCipher? cipher = null,
        int initialCapacity = DefaultCapacity)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
        _compressionThreshold = compressionThreshold;
        _cipher = cipher;
        _buffer = new PooledBufferWriter(Math.Max(initialCapacity, 1024));
    }

    /// <summary>Negative means no compression envelope. Fixed for the life of the writer.</summary>
    public int CompressionThreshold => _compressionThreshold;

    /// <summary>True when an encryptor is attached.</summary>
    public bool IsEncrypted => _cipher is not null;

    /// <summary>Bytes framed but not yet handed to the stream.</summary>
    public long UnflushedBytes
    {
        get
        {
            ThrowIfBroken();
            return _buffer.Length;
        }
    }

    /// <summary>Frames one packet — varint id plus body, already assembled by the caller.</summary>
    public void WritePacket(ReadOnlySpan<byte> packet)
    {
        ThrowIfBroken();
        var start = _buffer.Length;
        _buffer.WritePacket(packet, _compressionThreshold);
        Encrypt(start);
    }

    /// <summary>Frames one packet, putting the varint id in front of the body.</summary>
    public void WritePacket(int id, ReadOnlySpan<byte> body)
    {
        ThrowIfBroken();
        var start = _buffer.Length;
        _buffer.WritePacket(id, body, _compressionThreshold);
        Encrypt(start);
    }

    /// <summary>Frames one packet whose body is split across segments, without joining it first.</summary>
    public void WritePacket(int id, in ReadOnlySequence<byte> body)
    {
        ThrowIfBroken();
        var start = _buffer.Length;
        _buffer.WritePacket(id, in body, _compressionThreshold);
        Encrypt(start);
    }

    /// <summary>One write plus one stream flush. When it returns, the bytes are at the socket.</summary>
    public ValueTask FlushAsync(CancellationToken token = default)
    {
        ThrowIfBroken();
        if (_buffer.Length == 0)
        {
            token.ThrowIfCancellationRequested();
            return default;
        }

        return FlushCoreAsync(token);
    }

    /// <summary>Flushes whatever is left. After it the writer holds nothing.</summary>
    public ValueTask CompleteAsync() => FlushAsync(CancellationToken.None);

    private async ValueTask FlushCoreAsync(CancellationToken token)
    {
        Volatile.Write(ref _flushing, 1);
        try
        {
            await _stream.WriteAsync(_buffer.WrittenMemory, token).ConfigureAwait(false);
            await _stream.FlushAsync(token).ConfigureAwait(false);
            _buffer.Clear();
        }
        catch (Exception ex)
        {
            Break(ex);
            throw;
        }
        finally
        {
            Volatile.Write(ref _flushing, 0);
        }
    }

    private void Encrypt(int start)
    {
        if (_cipher is null) return;
        _cipher.Transform(_buffer.WrittenSpan[start..]);
    }

    private void Break(Exception reason)
    {
        _fault ??= reason;

        // an abandoned write may still be pinned by the operating system: drop the buffer
        // instead of handing it back to a pool that would lend it out again
        _buffer.Abandon();
    }

    private void ThrowIfBroken()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_fault is not null) ExceptionDispatchInfo.Capture(_fault).Throw();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (Volatile.Read(ref _flushing) == 1) _buffer.Abandon();
        else _buffer.Dispose();
    }
}
