using System.Buffers;
using System.IO.Pipelines;
using System.Security.Cryptography;
using DotNext.Buffers;
using McProtoNet.Net.Zlib;
using McProtoNet.Serialization;
using Org.BouncyCastle.Crypto;

namespace McProtoNet.Net;

public sealed class MinecraftPacketPipeWriter
{
    private readonly EncryptedPipeWriter _pipeWriter;

    public MinecraftPacketPipeWriter(PipeWriter pipeWriter)
    {
        _pipeWriter = new EncryptedPipeWriter(pipeWriter);
    }

    public bool EncryptionEnabled => _pipeWriter.IsEncrypted;

    public void EnableEncryption(IBufferedCipher decryptor)
    {
        _pipeWriter.SwitchEncryption(decryptor);
    }

    public void CancelPendingFlush()
    {
        _pipeWriter.CancelPendingFlush();
    }

    public ValueTask CompleteAsync(Exception? ex = null)
    {
        return _pipeWriter.CompleteAsync(ex);
    }

    public void Complete(Exception? ex = null)
    {
        _pipeWriter.Complete(ex);
    }

    public ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default) =>
        _pipeWriter.FlushAsync(cancellationToken);

    public int CompressionThreshold { get; set; }

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