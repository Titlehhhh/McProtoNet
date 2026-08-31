using System.Buffers;
using McProtoNet.Primitives;
using McProtoNet.Transport.Compression;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Provides a reader that takes exactly one frame at a time from a <see cref="Stream"/>.
/// </summary>
/// <remarks>
/// The length varint is read one byte at a time and the body by an exact read, so the reader never
/// takes a byte past the end of a frame. That is what allows <see cref="Cipher"/> and
/// <see cref="CompressionThreshold"/> to be switched between two frames. The returned body is a window
/// into a pooled buffer and stays valid only until the next read. Concurrent reads are not allowed.
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

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStreamReader"/> class that reads from the
    /// specified stream and rents its buffers from <see cref="ArrayPool{T}.Shared"/>.
    /// </summary>
    /// <param name="stream">The stream to read frames from.</param>
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open when the reader is
    /// disposed; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public PacketStreamReader(Stream stream, bool leaveOpen = false) :
        this(stream, ArrayPool<byte>.Shared, leaveOpen)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketStreamReader"/> class that reads from the
    /// specified stream and rents its buffers from the specified pool.
    /// </summary>
    /// <param name="stream">The stream to read frames from.</param>
    /// <param name="pool">The pool that packet buffers are rented from and returned to.</param>
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open when the reader is
    /// disposed; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.
    /// -or-
    /// <paramref name="pool"/> is <see langword="null"/>.</exception>
    public PacketStreamReader(Stream stream, ArrayPool<byte> pool, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(pool);
        _stream = stream;
        _pool = pool;
        _leaveOpen = leaveOpen;
    }

    /// <summary>
    /// Gets the stream that frames are read from.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _stream;
        }
    }

    /// <summary>
    /// Gets the cipher that decrypts every byte in place as it is read.
    /// </summary>
    /// <value>
    /// The decryptor, or <see langword="null"/> when the stream is not encrypted.
    /// </value>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <remarks>
    /// The cipher can be set only between two frames. The reader holds no bytes of the next frame, so
    /// the change always lands on a frame boundary.
    /// </remarks>
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

    /// <summary>
    /// Gets or sets the compression threshold, in bytes. A negative value disables the compression
    /// envelope.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">A read is in progress.</exception>
    /// <remarks>
    /// A new value applies from the next frame. Set this property between frames.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously reads exactly one frame from the stream.
    /// </summary>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the packet
    /// that was read.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="InvalidOperationException">Another read is already in progress.</exception>
    /// <exception cref="InvalidDataException">The frame length is not positive or is greater than 32
    /// MiB, the length varint is longer than five bytes, the declared uncompressed size is out of
    /// range, or decompression failed.</exception>
    /// <exception cref="EndOfStreamException">The stream ended in the middle of a frame.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
    /// <remarks>
    /// <see cref="IncomingPacket.Body"/> is a window into a pooled buffer and stays valid only until
    /// the next call. Cancellation after the read has started leaves the stream positioned inside a
    /// frame.
    /// </remarks>
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

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="PacketStreamReader"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// The buffer of the last packet is returned to the pool, which invalidates the body of that
    /// packet. The stream is disposed unless the reader was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>.
    /// </remarks>
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, Normal) == Disposed) return;

        ReturnBufferToPool();
        if (!_leaveOpen) _stream.Dispose();
    }

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="PacketStreamReader"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// The buffer of the last packet is returned to the pool, which invalidates the body of that
    /// packet. The stream is disposed unless the reader was created with <c>leaveOpen</c> set to
    /// <see langword="true"/>.
    /// </remarks>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
