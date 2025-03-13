using System.Buffers;
using System.Runtime.CompilerServices;
using McProtoNet.Abstractions;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

/// <summary>
/// Handles sending Minecraft protocol packets with optional compression
/// </summary>
public sealed class MinecraftPacketSender
{
    /// <summary>
    /// VarInt representing zero, used for uncompressed packets
    /// </summary>
    private static readonly byte[] ZERO_VARINT = { 0 };

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
                    var compressor = LibDeflateCache.RentCompressor();

                    var length = compressor.GetBound(uncompressedSize);
                    var compressedBuffer = ArrayPool<byte>.Shared.Rent(length);
                    try
                    {
                        var bytesCompress = compressor.Compress(data.Span, compressedBuffer.AsSpan(0, length));
                        var compressedLength = bytesCompress;

                        var fullSize = compressedLength + uncompressedSize.GetVarIntLength();

                        await SendCompress(fullSize, uncompressedSize, compressedBuffer, bytesCompress, cancellationToken);
                    }
                    catch
                    {
                        ArrayPool<byte>.Shared.Return(compressedBuffer);
                        throw;
                    }
                }

                uncompressedSize++;
                await SendShort(uncompressedSize, data, cancellationToken);
            }

            await SendPacketWithoutCompressionAsync(data, cancellationToken);
        }
        finally
        {
            await BaseStream.FlushAsync(cancellationToken);
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
        try
        {
            await BaseStream.WriteVarIntAsync(unSize, token).ConfigureAwait(false);
            await BaseStream.WriteAsync(ZERO_VARINT, token).ConfigureAwait(false);
            await BaseStream.WriteAsync(data, token).ConfigureAwait(false);
        }
        finally
        {
            await BaseStream.FlushAsync(token);
        }
    }

    /// <summary>
    /// Sends a compressed packet
    /// </summary>
    /// <param name="fullSize">The full packet size</param>
    /// <param name="uncompressedSize">The uncompressed data size</param>
    /// <param name="compressedBuffer">The compressed data buffer</param>
    /// <param name="bytesCompress">The number of compressed bytes</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>A ValueTask representing the send operation</returns>
    private async ValueTask SendCompress(int fullSize, int uncompressedSize, byte[] compressedBuffer, int bytesCompress,
        CancellationToken token)
    {
        try
        {
            await BaseStream.WriteVarIntAsync(fullSize, token).ConfigureAwait(false);
            await BaseStream.WriteVarIntAsync(uncompressedSize, token).ConfigureAwait(false);
            //await BaseStream.WriteAsync(compressed.Memory, token);

            await BaseStream.WriteAsync(compressedBuffer.AsMemory(0, bytesCompress), token)
                .ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(compressedBuffer);
        }
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

    /// <summary>
    /// Sends a packet without compression
    /// </summary>
    /// <param name="data">The packet data to send</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>A ValueTask representing the send operation</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private async ValueTask SendPacketWithoutCompressionAsync(ReadOnlyMemory<byte> data, CancellationToken token)
    {
        var len = data.Length;

        await BaseStream.WriteVarIntAsync(len, token).ConfigureAwait(false);

        await BaseStream.WriteAsync(data, token).ConfigureAwait(false);
    }

    #endregion
}