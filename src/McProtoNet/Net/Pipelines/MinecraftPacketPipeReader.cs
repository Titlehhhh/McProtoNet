using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using DotNext;
using DotNext.Buffers;
using DotNext.IO.Pipelines;
using McProtoNet.Abstractions;
using McProtoNet.Net.Zlib;
using LengthFormat = DotNext.IO.LengthFormat;

namespace McProtoNet.Net;

internal sealed class MinecraftPacketPipeReader
{
    private readonly PipeReader pipeReader;
    private static readonly MemoryAllocator<byte> s_allocator = ArrayPool<byte>.Shared.ToAllocator();

    public MinecraftPacketPipeReader(PipeReader pipeReader)
    {
        this.pipeReader = pipeReader;
        //this.decompressor = decompressor;
    }

    public int CompressionThreshold { get; set; }


    public async IAsyncEnumerable<InputPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var chunkcount = 0;
        cancellationToken.ThrowIfCancellationRequested();
        while (!cancellationToken.IsCancellationRequested)
        {
            ReadResult result = default;
            try
            {
                result = await pipeReader.ReadAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await pipeReader.CompleteAsync();
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
                pipeReader.AdvanceTo(buffer.Start, buffer.End);
            }
        }

        await pipeReader.CompleteAsync();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryReadPacket(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> packet)
    {
        scoped SequenceReader<byte> reader = new(buffer);


        packet = ReadOnlySequence<byte>.Empty;

        if (buffer.Length < 1) return false; // Недостаточно данных для чтения заголовка пакета

        int length;
        if (!reader.TryReadVarInt(out length, out _)) return false; // Невозможно прочитать длину заголовка


        if (length > reader.Remaining) return false; // Недостаточно данных для чтения полного пакета


        packet = reader.UnreadSequence.Slice(0, length);

        reader.Advance(length);


        buffer = buffer.Slice(reader.Position);

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InputPacket Decompress(in ReadOnlySequence<byte> data)
    {
        if (CompressionThreshold == -1)
        {
            return new InputPacket(data);
        }

        data.TryReadVarInt(out var sizeUncompressed, out var len);

        if (sizeUncompressed == 0)
        {
            return new InputPacket(data.Slice(1));
        }


        return new InputPacket(data.Slice(len).Decompress(sizeUncompressed));
    }
}