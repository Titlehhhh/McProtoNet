using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Transport.Compression;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
///     One frame per call over a <see cref="Stream" />: the length varint byte by byte, the body by
///     an exact read — never a byte past the frame. That precision is what lets the cipher and the
///     compression threshold be switched between two frames.
/// </summary>
/// <remarks>
///     The returned body is a window into a pooled buffer and stays valid only until the next read.
/// </remarks>
public sealed class PacketStreamReader : IDisposable, IAsyncDisposable
{
    private const int NotRead = 0;
    private const int Reading = 1;

    private const int Normal = 0;
    private const int Disposed = 1;

    private readonly Stream _stream;
    private readonly ArrayPool<byte> _pool;
    private readonly bool _leaveOpen;
    private readonly byte[] _varIntBuff = new byte[1];

    private volatile byte[]? _bytes;
    private volatile int _compressionThreshold = -1;
    private volatile int _readState = NotRead;
    private volatile int _state;

    private PacketCipher? _cipher;

    public PacketStreamReader(Stream stream, bool leaveOpen = false) :
        this(stream, ArrayPool<byte>.Shared, leaveOpen)
    {
    }

    public PacketStreamReader(Stream stream, ArrayPool<byte> pool, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(pool);
        _stream = stream;
        _pool = pool;
        _leaveOpen = leaveOpen;
    }

    /// <summary>The stream frames are read from.</summary>
    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _stream;
        }
    }

    /// <summary>
    ///     Decryptor applied in place to every byte as it is read. Set it between two frames —
    ///     the reader never holds bytes of the next frame, so the switch lands on a frame boundary.
    /// </summary>
    public PacketCipher? Cipher
    {
        get
        {
            ThrowIfDisposed();
            return _cipher;
        }
        internal set
        {
            ThrowIfDisposed();
            if (_readState == Reading)
                throw new InvalidOperationException("Cannot change the cipher while reading a packet");

            _cipher = value;
        }
    }

    /// <summary>Negative means no compression envelope. A change takes effect from the next frame.</summary>
    public int CompressionThreshold
    {
        get
        {
            ThrowIfDisposed();
            return _compressionThreshold;
        }
        set
        {
            ThrowIfDisposed();
            if (_readState == Reading)
                throw new InvalidOperationException("Cannot set CompressionThreshold while reading a packet");

            _compressionThreshold = value;
        }
    }

    /// <summary>Reads exactly one frame. The body window is valid until the next call.</summary>
    public async ValueTask<IncomingPacket> ReadPacketAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (Interlocked.CompareExchange(ref _readState, Reading, NotRead) == Reading)
            ThrowHelper.ThrowConcurrentRead();

        ReturnBufferToPool();

        try
        {
            var len = await ReadLengthAsync(token).ConfigureAwait(false);
            if (len <= 0 || len > BufferedPacketReader.MaxFrameLength) ThrowHelper.ThrowInvalidFrameLength(len);

            var buffer = _pool.Rent(len);
            Memory<byte> memory = buffer.AsMemory(0, len);
            try
            {
                await _stream.ReadExactlyAsync(memory, token).ConfigureAwait(false);
                _cipher?.Transform(memory.Span);

                if (_compressionThreshold < 0)
                    return CreatePacket(buffer, memory);

                var sizeUncompressed = memory.Span.ReadVarInt(out var offsetSizeUncompressed);

                if (sizeUncompressed <= 0)
                    return CreatePacket(buffer, memory[offsetSizeUncompressed..]);

                if (sizeUncompressed > BufferedPacketReader.MaxFrameLength)
                    ThrowHelper.ThrowInvalidUncompressedSize(sizeUncompressed);

                var decompressed = _pool.Rent(sizeUncompressed);
                try
                {
                    var decMem = decompressed.AsMemory(0, sizeUncompressed);
                    DecompressCore(memory.Span[offsetSizeUncompressed..], decMem.Span);

                    _pool.Return(buffer);
                    return CreatePacket(decompressed, decMem);
                }
                catch
                {
                    _pool.Return(decompressed);
                    throw;
                }
            }
            catch
            {
                _pool.Return(buffer);
                throw;
            }
        }
        finally
        {
            Interlocked.Exchange(ref _readState, NotRead);
        }
    }

    private async ValueTask<int> ReadLengthAsync(CancellationToken token)
    {
        var memory = _varIntBuff.AsMemory(0, 1);
        var numRead = 0;
        var result = 0;
        byte read;
        do
        {
            await _stream.ReadExactlyAsync(memory, token).ConfigureAwait(false);
            _cipher?.Transform(memory.Span);

            read = memory.Span[0];
            result |= (read & 0b01111111) << (7 * numRead);

            numRead++;
            if (numRead > 5) ThrowHelper.ThrowVarIntTooLong();
        } while ((read & 0b10000000) != 0);

        return result;
    }

    internal static void DecompressCore(ReadOnlySpan<byte> bufferCompress, Span<byte> uncompress)
    {
        var decompressor = LibDeflateCache.RentDecompressor();
        var status = decompressor.Decompress(bufferCompress, uncompress, out var written);

        // status first: a broken frame reports its own reason, not a buffer-length mismatch
        if (status != OperationStatus.Done) ThrowHelper.ThrowDecompressFailed(status);
        if (written != uncompress.Length) ThrowHelper.ThrowDecompressSizeMismatch(written, uncompress.Length);
    }

    private void ReturnBufferToPool()
    {
        var old = Interlocked.Exchange(ref _bytes, null);
        if (old is not null) _pool.Return(old);
    }

    private IncomingPacket CreatePacket(byte[] pooledArr, Memory<byte> readData)
    {
        var old = Interlocked.Exchange(ref _bytes, pooledArr);
        if (old is not null) _pool.Return(old);

        var id = readData.Span.ReadVarInt(out var len);
        return new IncomingPacket(id, readData[len..]);
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_state == Disposed, typeof(PacketStreamReader));

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, Normal) == Disposed) return;

        ReturnBufferToPool();
        if (!_leaveOpen) _stream.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
