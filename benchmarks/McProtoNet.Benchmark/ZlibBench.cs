using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using DotNext.Buffers;
using DotNext.Collections.Generic;
using DotNext.IO;
using McProtoNet.Net.Zlib;
using ZlibNGSharpMinimal.Deflate;
using ZlibNGSharpMinimal.Inflate;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace McProtoNet.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ZlibBench
{
    private byte[] compressed1;
    private byte[] compressed2;
    private byte[] outTest;

    private ReadOnlySequence<byte> sec;
    [Params(100, 500)] public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        outTest = new byte[DataLength];
        Random r = new Random(27);
        byte[] testData = new byte[DataLength];
        r.NextBytes(testData);

        MemoryStream ms = new MemoryStream();
        using (var zlib = new ZLibStream(ms, CompressionLevel.Optimal, true))
        {
            zlib.Write(testData);
        }

        var compressed = ms.ToArray();
        int a = 10;

        compressed1 = compressed.AsSpan().Slice(0, a).ToArray();
        compressed2 = compressed.AsSpan().Slice(a).ToArray();
        SequenceBuilder<byte> builder = new SequenceBuilder<byte>();

        builder.Write(compressed1);
        builder.Write(compressed2);

        sec = builder.ToReadOnlySequence();
        builder.Dispose();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
    }

    private static MemoryAllocator<byte> s_allocator = ArrayPool<byte>.Shared.ToAllocator();

    [Benchmark]
    public void LibDeflateTest()
    {
        //MemoryOwner<byte> tempBuffer = s_allocator.AllocateExactly(DataLength);
        
         scoped var compressedTemp = DataLength <= 256
             ? new SpanOwner<byte>(stackalloc byte[DataLength])
             : new SpanOwner<byte>(DataLength);

        scoped SpanWriter<byte> sWriter = new SpanWriter<byte>(compressedTemp.Span);
        sWriter.Write(compressed1);
        sWriter.Write(compressed2);


        scoped ZlibDecompressor decompressor = new ZlibDecompressor();
        decompressor.Decompress(compressedTemp.Span, outTest, out _);

        decompressor.Dispose();
        compressedTemp.Dispose();
    }

    //[Benchmark]
    public void ZlibNg()
    {
        //Decompressor.Decompress(sec, outTest.AsSpan(0, DataLength));
        // using ZngInflater inflater = new ZngInflater();
        // inflater.Inflate(compressed1, outTest.AsSpan(0, DataLength));
        //hh.AsStream()
    }
}

internal static class Decompressor
{
    private static ThreadLocal<ZLibStream> s_stream = new ThreadLocal<ZLibStream>();
    private static ThreadLocal<ReadOnlyMemoryStream> m_stream = new ThreadLocal<ReadOnlyMemoryStream>();

    public static void Decompress(ReadOnlySequence<byte> input, Span<byte> outBytes)
    {
        if (!s_stream.IsValueCreated)
        {
            var ms = new ReadOnlyMemoryStream();
            m_stream.Value = ms;
            s_stream.Value = new ZLibStream(ms, CompressionMode.Decompress);
        }

        m_stream.Value.SetMemory(input);
        ZLibStream zlib = s_stream.Value;

        zlib.Read(outBytes);
        zlib.Flush();
    }
}

internal sealed class ReadOnlyMemoryStream : Stream
{
    public ReadOnlyMemoryStream()
    {
        this.sequence = ReadOnlySequence<byte>.Empty;
    }

    private ReadOnlySequence<byte> sequence;
    private SequencePosition position;

    public void SetMemory(ReadOnlySequence<byte> sec)
    {
        this.sequence = sec;
        position = sequence.Start;
    }


    private ReadOnlySequence<byte> RemainingSequence => sequence.Slice(position);

    public override bool CanRead => true;
    public override bool CanSeek => true;

    public override bool CanWrite { get; }
    public override long Length => sequence.Length;

    public override long Position
    {
        get => sequence.GetOffset(position);
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan((ulong)value, (ulong)sequence.Length, nameof(value));

            position = sequence.GetPosition(value);
        }
    }

    public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken token)
    {
        ValidateCopyToArguments(destination, bufferSize);

        foreach (var segment in RemainingSequence)
            await destination.WriteAsync(segment, token).ConfigureAwait(false);

        position = sequence.End;
    }

    public override void CopyTo(Stream destination, int bufferSize)
    {
        ValidateCopyToArguments(destination, bufferSize);

        foreach (var segment in RemainingSequence)
            destination.Write(segment.Span);

        position = sequence.End;
    }

    public override void SetLength(long value)
    {
        var newSeq = sequence.Slice(0L, value);
        position = newSeq.GetPosition(Math.Min(sequence.GetOffset(position), newSeq.Length));
        sequence = newSeq;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override void Flush()
    {
        //throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }

    public override int Read(Span<byte> buffer)
    {
        RemainingSequence.CopyTo(buffer, out var writtenCount);
        position = sequence.GetPosition(writtenCount, position);
        return writtenCount;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        var newPosition = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => sequence.GetOffset(position) + offset,
            SeekOrigin.End => sequence.Length + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };

        if (newPosition < 0L)
            throw new IOException();

        ArgumentOutOfRangeException.ThrowIfGreaterThan(newPosition, sequence.Length, nameof(offset));

        position = sequence.GetPosition(newPosition);
        return newPosition;
    }

    public override string ToString() => sequence.ToString();
}