using System.Buffers;
using System.Threading.Tasks.Sources;
using McProtoNet.Internal;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

/// <summary>
/// Reads Minecraft protocol packets from a stream, handling compression if enabled
/// </summary>
public sealed class MinecraftPacketReader : IDisposable, IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly ArrayPool<byte> _pool;
    private readonly bool _leaveOpen;
    private readonly byte[] _varIntBuff = new byte[1];

    private volatile byte[]? _bytes;

    private volatile int _compressionThreshold = -1;

    private volatile int _readState = NotRead;
    private volatile int _state;

    private readonly PacketSourceCore _sourceCore = new();

    private const int NotRead = 0;
    private const int Reading = 1;

    private const int Normal = 0;
    private const int Disposed = 1;


    public MinecraftPacketReader(Stream stream, bool leaveOpen = false) :
        this(stream, ArrayPool<byte>.Shared, leaveOpen)
    {
    }

    public MinecraftPacketReader(Stream stream, ArrayPool<byte> pool, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(pool);
        _stream = stream;
        _pool = pool;
        _leaveOpen = leaveOpen;
    }


    /// <summary>
    /// Gets the underlying stream to read packets from
    /// </summary>
    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _stream;
        }
    }


    /// <summary>
    /// Reads the next packet from the stream asynchronously
    /// </summary>
    /// <param name="token">Cancellation token to cancel the operation</param>
    /// <returns>The read packet data</returns>
    /// <exception cref="Exception">Thrown when decompression fails or packet size is invalid</exception>
    public async ValueTask<NewInputPacket> ReadPacketAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (Interlocked.CompareExchange(ref _readState, Reading, NotRead) == Reading)
        {
            throw new InvalidOperationException("Concurrent packet reading is not allowed.");
        }

        ReturnBufferToPool();

        var len = await BaseStream.ReadVarIntAsync(_varIntBuff, token)
            .ConfigureAwait(false);

        var buffer = _pool.Rent(len);

        Memory<byte> memory = buffer.AsMemory(0, len);
        try
        {
            await BaseStream.ReadExactlyAsync(memory, token)
                .ConfigureAwait(false);

            if (_compressionThreshold < 0)
            {
                return CreatePacket(buffer, memory);
            }

            var sizeUncompressed = memory.Span.ReadVarInt(out var offsetSizeUncompressed);

            if (sizeUncompressed <= 0)
            {
                return CreatePacket(buffer, memory[offsetSizeUncompressed..]);
            }


            var decompressed = _pool.Rent(sizeUncompressed);
            try
            {
                var decMem = decompressed.AsMemory(0, sizeUncompressed);
                DecompressCore(memory.Span[offsetSizeUncompressed..],
                    decMem.Span);

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
        finally
        {
            Interlocked.Exchange(ref _readState, NotRead);
        }
    }

    private static void DecompressCore(ReadOnlySpan<byte> bufferCompress, Span<byte> uncompress)
    {
        var decompressor = LibDeflateCache.RentDecompressor();
        var status = decompressor.Decompress(
            bufferCompress,
            uncompress, out var written);

        if (written != uncompress.Length)
            throw new InvalidOperationException("Written not equal uncompress buffer length");

        switch (status)
        {
            case OperationStatus.InvalidData:
                throw new InvalidDataException("Decompress Error: Invalid Data");
            case OperationStatus.NeedMoreData:
                throw new InvalidOperationException("Decompress Error: Need more data");
            case OperationStatus.DestinationTooSmall:
                throw new InvalidOperationException("Decompress Error: Destination buffer too small");
            case OperationStatus.Done:
                break;
            default:
                throw new InvalidOperationException($"Decompress Error: {status}");
        }
    }

    private void ReturnBufferToPool()
    {
        var old = Interlocked.Exchange(ref _bytes, null);
        if (old is not null)
        {
            _sourceCore.Reset();
            _pool.Return(old);
        }
    }

    private NewInputPacket CreatePacket(byte[] pooledArr, Memory<byte> readData)
    {
        var old = Interlocked.Exchange(ref _bytes, pooledArr);
        if (old is not null)
        {
            _pool.Return(old);
        }

        var id = readData.Span.ReadVarInt(out var len);
        var data = new ReadOnlySequence<byte>(readData[len..]);

        _sourceCore.Id = id;
        _sourceCore.Data = data;

        return new NewInputPacket(_sourceCore, _sourceCore.Version);
    }

    /// <summary>
    ///     Get the compression threshold for this packet reader.
    ///     Any packet larger than this threshold will be compressed.
    /// </summary>
    /// <value>
    ///     The compression threshold in bytes.
    /// </value>
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
            {
                throw new InvalidOperationException("Cannot set CompressionThreshold while reading a packet");
            }

            _compressionThreshold = value;
        }
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_state == Disposed, typeof(MinecraftPacketReader));


    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _state, Disposed, Normal) == Disposed)
        {
            return;
        }

        ReturnBufferToPool();
        if (!_leaveOpen)
        {
            _stream.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}