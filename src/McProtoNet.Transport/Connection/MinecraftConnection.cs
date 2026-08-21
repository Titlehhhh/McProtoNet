using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Transport;

/// <summary>
///     A connection in one-at-a-time mode: one frame per read, one frame per write, nothing held
///     back. This is where the switches live — the compression threshold and encryption — because
///     nothing of the next frame is ever buffered, so a switch always lands between two frames.
///     Handshaking, status and login live here; <see cref="ToStreaming" /> moves the rest of the
///     connection to <see cref="StreamingConnection" />.
/// </summary>
/// <remarks>
///     One reader, one writer, no queue inside: send policy belongs to the caller.
///     <see cref="Abort" /> may be called from any thread.
/// </remarks>
public sealed class MinecraftConnection : IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly bool _leaveOpen;
    private readonly PacketStreamReader _reader;
    private readonly PacketStreamWriter _writer;

    private PacketCipher? _encryptor;
    private PacketCipher? _decryptor;
    private int _threshold = -1;
    private int _moved;
    private int _closed;
    private Exception? _closeReason;

    public MinecraftConnection(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
        _leaveOpen = leaveOpen;
        _reader = new PacketStreamReader(stream, leaveOpen: true);
        _writer = new PacketStreamWriter(stream, leaveOpen: true);
    }

    /// <summary>The stream underneath — the raw escape hatch for proxies and replays.</summary>
    public Stream BaseStream => _stream;

    /// <summary>Negative means no compression envelope. A change takes effect from the next frame.</summary>
    public int CompressionThreshold
    {
        get
        {
            ThrowIfUnusable();
            return _threshold;
        }
        set
        {
            ThrowIfUnusable();
            _threshold = value;
            _reader.CompressionThreshold = value;
            _writer.CompressionThreshold = value;
        }
    }

    /// <summary>True once <see cref="EnableEncryption" /> has run.</summary>
    public bool IsEncrypted => _encryptor is not null;

    /// <summary>
    ///     Turns on AES/CFB8 in both directions from the next frame on. Call it right after the frame
    ///     that agreed the secret has been written and before the next one is read.
    /// </summary>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        ThrowIfUnusable();
        if (_encryptor is not null)
            throw new InvalidOperationException("Encryption is already enabled.");

        _encryptor = PacketCipher.CreateEncryptor(sharedSecret);
        _decryptor = PacketCipher.CreateDecryptor(sharedSecret);
        _reader.Cipher = _decryptor;
        _writer.Cipher = _encryptor;
    }

    /// <summary>Reads exactly one frame. The body window is valid until the next read.</summary>
    public async ValueTask<IncomingPacket> ReadPacketAsync(CancellationToken token = default)
    {
        ThrowIfUnusable();
        try
        {
            return await _reader.ReadPacketAsync(token).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsAborted(ex))
        {
            throw new ConnectionAbortedException(Volatile.Read(ref _closeReason));
        }
    }

    /// <summary>Writes one packet and flushes. When it returns, the bytes are at the socket.</summary>
    public async ValueTask WritePacketAsync(ReadOnlyMemory<byte> packet, CancellationToken token = default)
    {
        ThrowIfUnusable();
        try
        {
            await _writer.WritePacketAsync(packet, token).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsAborted(ex))
        {
            throw new ConnectionAbortedException(Volatile.Read(ref _closeReason));
        }
    }

    /// <summary>Writes one packet, putting the varint id in front of the body, and flushes.</summary>
    public async ValueTask WritePacketAsync(int id, ReadOnlyMemory<byte> body, CancellationToken token = default)
    {
        ThrowIfUnusable();
        try
        {
            await _writer.WritePacketAsync(id, body, token).ConfigureAwait(false);
        }
        catch (Exception ex) when (IsAborted(ex))
        {
            throw new ConnectionAbortedException(Volatile.Read(ref _closeReason));
        }
    }

    /// <summary>
    ///     Consumes this connection and hands the stream, the ciphers and the threshold to a
    ///     streaming one. Afterwards every member here throws and <see cref="DisposeAsync" /> is empty.
    /// </summary>
    /// <remarks>
    ///     Like a read, this invalidates the body of the last packet <see cref="ReadPacketAsync" />
    ///     returned — decode it before moving.
    /// </remarks>
    public StreamingConnection ToStreaming()
    {
        ThrowIfUnusable();
        if (Interlocked.Exchange(ref _moved, 1) == 1)
            throw new InvalidOperationException("This connection was already moved to streaming mode.");

        var streaming = new StreamingConnection(_stream, _leaveOpen, _threshold, _encryptor, _decryptor);
        _reader.Dispose();
        _writer.Dispose();

        // Abort may have landed between the check and the move: the new object inherits the close
        if (Volatile.Read(ref _closed) == 1) streaming.Abort(Volatile.Read(ref _closeReason));
        return streaming;
    }

    /// <summary>Closes the stream from any thread; an in-flight read or write fails with the reason.</summary>
    public void Abort(Exception? reason = null)
    {
        if (Volatile.Read(ref _closed) == 1) return;

        // reason first: a thread that sees _closed == 1 must also see why
        Interlocked.CompareExchange(ref _closeReason, reason, null);
        if (Interlocked.Exchange(ref _closed, 1) == 1) return;

        CloseStream();
    }

    private void CloseStream()
    {
        try
        {
            _stream.Dispose();
        }
        catch
        {
        }
    }

    public ValueTask DisposeAsync()
    {
        if (Volatile.Read(ref _moved) == 1) return ValueTask.CompletedTask;

        if (Interlocked.Exchange(ref _closed, 1) == 0 && !_leaveOpen) CloseStream();
        _reader.Dispose();
        _writer.Dispose();
        _encryptor?.Dispose();
        _decryptor?.Dispose();
        return ValueTask.CompletedTask;
    }

    private bool IsAborted(Exception ex) =>
        ex is not OperationCanceledException && Volatile.Read(ref _closed) == 1;

    private void ThrowIfUnusable()
    {
        if (Volatile.Read(ref _moved) == 1)
            throw new InvalidOperationException(
                "This connection was moved to streaming mode; use the StreamingConnection instead.");

        if (Volatile.Read(ref _closed) == 1)
            throw new ConnectionAbortedException(Volatile.Read(ref _closeReason));
    }
}
