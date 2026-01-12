using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;

namespace McProtoNet.Net;

public sealed class MinecraftPacketPipeWriter : PipeWriter
{
    private readonly EncryptedPipeWriter _pipeWriter;

    public MinecraftPacketPipeWriter(PipeWriter pipeWriter)
    {
        _pipeWriter = new EncryptedPipeWriter(pipeWriter);
    }

    public bool EncryptionEnabled => _pipeWriter.IsEncrypted;

    public int CompressionThreshold { get; set; } = -1;

    public void EnableEncryption(IBufferedCipher decryptor)
    {
        _pipeWriter.SwitchEncryption(decryptor);
    }

    public override void CancelPendingFlush()
    {
        _pipeWriter.CancelPendingFlush();
    }

    public override ValueTask CompleteAsync(Exception? exception = null)
    {
        return _pipeWriter.CompleteAsync(exception);
    }

    public override void Complete(Exception? exception = null)
    {
        _pipeWriter.Complete(exception);
    }

    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        return _pipeWriter.FlushAsync(cancellationToken);
    }


    public override void Advance(int bytes)
    {
        _pipeWriter.Advance(bytes);
    }

    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        return _pipeWriter.GetMemory(sizeHint);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        return _pipeWriter.GetSpan(sizeHint);
    }

    public MinecraftPrimitiveWriter GetWriter(int size = 0)
    {
        throw null;
        // if (CompressionThreshold < 0)
        // {
        //     return new MinecraftPrimitiveWriter(GetSpan(size));
        // }
        // else
        // {
        //    //var writer =  
        // }
    }

    public void WritePacket(ReadOnlySpan<byte> rawPacket)
    {
        if (CompressionThreshold < 0)
        {
            _pipeWriter.WriteVarInt(rawPacket.Length);
            _pipeWriter.Write(rawPacket);
        }
        else
        {
            if (rawPacket.Length < CompressionThreshold)
            {
                _pipeWriter.WriteVarInt(rawPacket.Length + 1);
                _pipeWriter.WriteVarInt(0);
                _pipeWriter.Write(rawPacket);
            }
            else
            {
                var uncompressedSize = rawPacket.Length;
                var compressor = LibDeflateCache.RentCompressor();
                var length = compressor.GetBound(uncompressedSize);

                // TODO: Use _pipeWriter.GetSpan()
                var compressedBuffer = ArrayPool<byte>.Shared.Rent(length);

                try
                {
                    var bytesCompress =
                        compressor.Compress(rawPacket, compressedBuffer.AsSpan(0, length));

                    var compressedLength = bytesCompress;

                    var fullsize = compressedLength + uncompressedSize.GetVarIntLength();

                    _pipeWriter.WriteVarInt(fullsize);
                    _pipeWriter.WriteVarInt(uncompressedSize);
                    _pipeWriter.Write(compressedBuffer.AsSpan(0, bytesCompress));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(compressedBuffer);
                }
            }
        }
    }
}