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


    private static void DecompressMemory(ReadOnlySpan<byte> compressed, Span<byte> decompressed)
    {
        scoped var decompressor = new ZlibDecompressor();
        try
        {
            var result = decompressor.Decompress(
                compressed,
                decompressed,
                out var written);

            if (result != OperationStatus.Done)
                throw new Exception("Zlib: " + result);
        }
        finally
        {
            decompressor.Dispose();
        }
    }

    public async ValueTask<InputPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        MemoryOwner<byte> data = default;
        try
        {
            data =
                await pipeReader.ReadAsync(LengthFormat.Compressed, s_allocator, cancellationToken);
        }
        catch (Exception e)
        {
            data.Dispose();
            await pipeReader.CompleteAsync(e).ConfigureAwait(false);
            throw;
        }

        if (data.Length == 0)
        {
            data.Dispose();
        }

        if (CompressionThreshold == -1)
        {
            return new InputPacket(data);
        }

        int sizeUncomressed = data.Span.ReadVarInt(out int offset);
        if (sizeUncomressed == 0)
        {
            return new InputPacket(data, offset: 1);
        }

        try
        {
            Memory<byte> compressed = data.Memory.Slice(offset);

            MemoryOwner<byte> decompressed = s_allocator.AllocateExactly(sizeUncomressed);
            DecompressMemory(compressed.Span, decompressed.Span);

            return new InputPacket(decompressed);
        }
        finally
        {
            data.Dispose();
        }
    }

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
                    //consumed = buffer.Start;
                    //examined = consumed;
                    yield return Decompress(packet);
                }
            }
            finally
            {
                pipeReader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
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
        
        

        throw new Exception();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySequence<byte> DecompressMultiSegment(ReadOnlySequence<byte> compressed, byte[] decompressed,
        scoped ZlibDecompressor decompressor, int sizeUncompressed, out int id)
    {
        var compressedLength = (int)compressed.Length;

        using scoped var compressedTemp = compressedLength <= 256
            ? new SpanOwner<byte>(stackalloc byte[compressedLength])
            : new SpanOwner<byte>(compressedLength);

        scoped var decompressedSpan = decompressed.AsSpan(0, sizeUncompressed);

        scoped var compressedTempSpan = compressedTemp.Span;


        compressed.CopyTo(compressedTempSpan);


        var result = decompressor.Decompress(
            compressedTempSpan,
            decompressedSpan,
            out var written);

        if (result != OperationStatus.Done)
            throw new Exception("Zlib: " + sizeUncompressed);

        id = decompressedSpan.ReadVarInt(out var len);

        return new ReadOnlySequence<byte>(decompressed, len, sizeUncompressed - len);
    }
}