using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

/// <summary>
/// Reads Minecraft protocol packets from a stream, handling compression if enabled
/// </summary>
public sealed class MinecraftPacketReader
{
    private readonly byte[] _varIntBuff = new byte[1];

    private static readonly MemoryAllocator<byte> MemoryAllocator = ArrayPool<byte>.Shared.ToAllocator();

    /// <summary>
    /// The compression threshold in bytes. Values less than 0 indicate compression is disabled.
    /// </summary>
    private int _compressionThreshold = -1;

    /// <summary>
    /// Gets or sets the underlying stream to read packets from
    /// </summary>
    public Stream BaseStream { get; set; }

    /// <summary>
    /// Reads the next packet from the stream asynchronously
    /// </summary>
    /// <param name="token">Cancellation token to cancel the operation</param>
    /// <returns>The read packet data</returns>
    /// <exception cref="Exception">Thrown when decompression fails or packet size is invalid</exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public async ValueTask<InputPacket> ReadNextPacketAsync(CancellationToken token = default)
    {
        var len = await BaseStream.ReadVarIntAsync(_varIntBuff, token).ConfigureAwait(false);

        var buffer = MemoryAllocator.AllocateExactly(len);
        try
        {
            await BaseStream.ReadExactlyAsync(buffer.Memory, token).ConfigureAwait(false);

            if (_compressionThreshold < 0)
            {
                return new InputPacket(buffer);
            }

            var sizeUncompressed = buffer.Span.ReadVarInt(out var offsetSizeUncompressed);

            if (sizeUncompressed <= 0) return new InputPacket(buffer, offset: offsetSizeUncompressed);


            var memoryOwner = MemoryAllocator.AllocateExactly(sizeUncompressed);
            try
            {
                DecompressCore(buffer.Span[offsetSizeUncompressed..], memoryOwner.Span);
                buffer.Dispose();
                return new InputPacket(memoryOwner);
            }
            catch
            {
                memoryOwner.Dispose();
                throw;
            }
        }
        catch
        {
            buffer.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Decompresses data using LibDeflate
    /// </summary>
    /// <param name="bufferCompress">The compressed data buffer</param>
    /// <param name="uncompress">The buffer to store decompressed data</param>
    /// <exception cref="Exception">Thrown when decompression fails or output size is incorrect</exception>
    private static void DecompressCore(ReadOnlySpan<byte> bufferCompress, Span<byte> uncompress)
    {
        var decompressor = LibDeflateCache.RentDecompressor();
        var status = decompressor.Decompress(
            bufferCompress,
            uncompress, out var written);

        if (written != uncompress.Length)
            throw new Exception("Written not equal uncompress buffer length");

        if (status != OperationStatus.Done) throw new Exception("Decompress Error");
    }

    /// <summary>
    /// Enables or disables packet compression with the specified threshold
    /// </summary>
    /// <param name="threshold">The compression threshold in bytes. Values less than 0 disable compression.</param>
    public void SwitchCompression(int threshold)
    {
        _compressionThreshold = threshold;
    }
}