using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using DotNext;
using DotNext.Buffers;
using DotNext.IO.Pipelines;
using McProtoNet.Abstractions;
using McProtoNet.Net.Zlib;
using Org.BouncyCastle.Crypto;
using LengthFormat = DotNext.IO.LengthFormat;

namespace McProtoNet.Net;

internal sealed class MinecraftPacketPipeReader
{
    private DecryptedPipeReader _pipeReader;

    public MinecraftPacketPipeReader(PipeReader pipeReader)
    {
        this._pipeReader = new DecryptedPipeReader(pipeReader);
    }

    public int CompressionThreshold { get; set; }

    public bool EncryptionEnabled => _pipeReader.IsEncrypted;

    public void EnableEncryption(IBufferedCipher decryptor)
    {
        _pipeReader.SwitchEncryption(decryptor);
    }

    public async IAsyncEnumerable<NewInputPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chunkcount = 0;
        cancellationToken.ThrowIfCancellationRequested();
        while (!cancellationToken.IsCancellationRequested)
        {
            ReadResult result = default;
            try
            {
                result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await _pipeReader.CompleteAsync().ConfigureAwait(false);
                break;
            }

            var buffer = result.Buffer;
            if (result.IsCompleted) break;

            if (result.IsCanceled) break;


            try
            {
                while (TryReadPacket(ref buffer, out var packet))
                {
                    yield return Decompress(packet);
                }
            }
            finally
            {
                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
            }
        }

        await _pipeReader.CompleteAsync().ConfigureAwait(false);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadPacket(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> packet)
    {
        scoped SequenceReader<byte> reader = new(buffer);


        packet = ReadOnlySequence<byte>.Empty;

        if (buffer.Length < 1) return false; // Not enough data to read packet header

        if (!reader.TryReadVarInt(out var length, out _)) return false; // Unable to read packet length

        if (length > reader.Remaining) return false; // Not enough data to read full packet


        packet = reader.UnreadSequence.Slice(0, length);

        reader.Advance(length);


        buffer = buffer.Slice(reader.Position);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NewInputPacket Decompress(in ReadOnlySequence<byte> data)
    {
        if (CompressionThreshold == -1)
        {
            //Без сжатия
            return new NewInputPacket(data);
        }

        data.TryReadVarInt(out var sizeUncompressed, out var len);

        if (sizeUncompressed == 0)
        {
            // Со сжатием, короткий пакет
            return new NewInputPacket(data.Slice(1));
        }

        // Со сжатием, длинный пакет
        return new NewInputPacket(data.Slice(len).Decompress(sizeUncompressed));
    }
}

public struct NewInputPacket : IDisposable
{
    public int Id { get; }
    public ReadOnlySequence<byte> Data { get; }

    private readonly MemoryOwner<byte>? _memoryOwner;

    public NewInputPacket(ReadOnlySequence<byte> data)
    {
        data.TryReadVarInt(out var value, out var offset);
        Id = value;
        Data = data.Slice(offset);
    }


    /// <summary>
    /// Constructor for compressed packet
    /// </summary>
    /// <param name="owner"></param>
    public NewInputPacket(MemoryOwner<byte> owner)
    {
        _memoryOwner = owner;
        var data = new ReadOnlySequence<byte>(owner.Memory);
        data.TryReadVarInt(out var value, out var offset);
        Id = value;
        Data = data.Slice(offset);
    }

    public void Dispose()
    {
        _memoryOwner?.Dispose();
    }
}