using System.Buffers;
using System.Runtime.CompilerServices;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

/// <summary>
/// Handles sending Minecraft protocol packets with optional compression
/// </summary>
public sealed class MinecraftPacketSender
{
    private readonly byte[] _varIntBuff = new byte[5];
    private static readonly MemoryAllocator<byte> MemoryAllocator = ArrayPool<byte>.Shared.ToAllocator();


    public bool AutoFlush { get; set; } = true;

    /// <summary>
    /// VarInt representing zero, used for uncompressed packets
    /// </summary>
    private static readonly byte[] ZERO_VARINT = [0];

    /// <summary>
    /// The compression threshold in bytes. Values less than 0 indicate compression is disabled.
    /// </summary>
    private int _compressionThreshold = -1;

    /// <summary>
    /// Gets or sets the underlying stream to send packets to
    /// </summary>
    public Stream BaseStream { get; set; }

    /// <summary>
    /// Sends a packet asynchronously with optional compression
    /// </summary>
    /// <param name="data">The packet data to send</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A ValueTask representing the asynchronous operation</returns>
    public async ValueTask SendPacketAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_compressionThreshold >= 0)
            {
                var uncompressedSize = data.Length;

                if (uncompressedSize >= _compressionThreshold)
                {
                    using var compressedBuffer = Compress(data.Span);

                    int fullSize = compressedBuffer.Length + uncompressedSize.GetVarIntLength();

                    await BaseStream.WriteVarIntAsync(
                        fullSize, 
                        _varIntBuff,
                        cancellationToken).ConfigureAwait(false);
                    
                    
                    await BaseStream.WriteVarIntAsync(
                            uncompressedSize, 
                            _varIntBuff, 
                            cancellationToken)
                        .ConfigureAwait(false);

                    await BaseStream.WriteAsync(compressedBuffer.Memory, cancellationToken)
                        .ConfigureAwait(false);

                    return;
                }

                uncompressedSize++;
                await SendShort(uncompressedSize, data, cancellationToken).ConfigureAwait(false);
                return;
            }

            await SendPacketWithoutCompressionAsync(data, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (AutoFlush)
                await BaseStream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Sends a short uncompressed packet
    /// </summary>
    /// <param name="unSize">The uncompressed size</param>
    /// <param name="data">The packet data</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>A ValueTask representing the send operation</returns>
    private async ValueTask SendShort(int unSize, ReadOnlyMemory<byte> data, CancellationToken token)
    {
        await BaseStream.WriteVarIntAsync(unSize,_varIntBuff, token).ConfigureAwait(false);
        await BaseStream.WriteAsync(ZERO_VARINT, token).ConfigureAwait(false);
        await BaseStream.WriteAsync(data, token).ConfigureAwait(false);
    }


    /// <summary>
    /// Enables or disables packet compression with the specified threshold
    /// </summary>
    /// <param name="threshold">The compression threshold in bytes. Values less than 0 disable compression.</param>
    public void SwitchCompression(int threshold)
    {
        _compressionThreshold = threshold;
    }

    #region Send

    /// <summary>
    /// Sends an OutputPacket asynchronously
    /// </summary>
    /// <param name="packet">The packet to send</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A ValueTask representing the send operation</returns>
    public ValueTask SendPacketAsync(OutputPacket packet, CancellationToken cancellationToken = default)
    {
        return SendPacketAsync(packet.Memory, cancellationToken);
    }


    
    private async ValueTask SendPacketWithoutCompressionAsync(ReadOnlyMemory<byte> data, CancellationToken token)
    {
        var len = data.Length;

        await BaseStream.WriteVarIntAsync(len,_varIntBuff, token).ConfigureAwait(false);

        await BaseStream.WriteAsync(data, token).ConfigureAwait(false);
    }

    #endregion

    private static MemoryOwner<byte> Compress(ReadOnlySpan<byte> data)
    {
        var compressor = LibDeflateCache.RentCompressor();
        var length = compressor.GetBound(data.Length);

        var compressedBuffer = MemoryAllocator.AllocateExactly(length);
        try
        {
            var bytesCompress = compressor.Compress(data, compressedBuffer.Span);


            compressedBuffer.Resize(bytesCompress, MemoryAllocator);

            return compressedBuffer;
        }
        catch
        {
            compressedBuffer.Dispose();
            throw;
        }
    }
    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        await BaseStream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}