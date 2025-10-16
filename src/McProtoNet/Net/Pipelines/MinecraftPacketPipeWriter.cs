using System.Buffers;
using System.IO.Pipelines;
using System.Security.Cryptography;
using DotNext.Buffers;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

internal sealed class MinecraftPacketPipeWriter
{
    private static readonly byte[] ZeroVarInt = { 0 };

    private readonly PipeWriter pipeWriter;
    private readonly ICryptoTransform cryptoTransform;

    public MinecraftPacketPipeWriter(PipeWriter pipeWriter)
    {
        this.pipeWriter = pipeWriter;
    }

    public ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default) =>
        pipeWriter.FlushAsync(cancellationToken);

    public int CompressionThreshold { get; set; }

    public void WritePacket(ReadOnlySpan<byte> rawPacket)
    {
        if (CompressionThreshold < 0)
        {
            pipeWriter.WriteVarInt(rawPacket.Length);
            pipeWriter.Write(rawPacket);
        }
        else
        {
            if (rawPacket.Length < CompressionThreshold)
            {
                pipeWriter.WriteVarInt(rawPacket.Length + 1);
                pipeWriter.WriteVarInt(0);
                pipeWriter.Write(rawPacket);
            }
            else
            {
                var uncompressedSize = rawPacket.Length;
                var compressor = LibDeflateCache.RentCompressor();
                var length = compressor.GetBound(uncompressedSize);
                
                var compressedBuffer = ArrayPool<byte>.Shared.Rent(length);
                
                try
                {
                    var bytesCompress = 
                        compressor.Compress(rawPacket, compressedBuffer.AsSpan(0, length));
                
                    var compressedLength = bytesCompress;
                
                    var fullsize = compressedLength + uncompressedSize.GetVarIntLength();
                
                    pipeWriter.WriteVarInt(fullsize);
                    pipeWriter.WriteVarInt(uncompressedSize);
                    pipeWriter.Write(compressedBuffer.AsSpan(0, bytesCompress));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(compressedBuffer);
                }
                
            }
        }

        
        
    }

}