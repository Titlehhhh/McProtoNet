using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net;

namespace McProtoNet.Benchmark;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class ReadPacketBenchmarks
{
    [Params(3_000_000)] public int PacketsCount;

    [Params(-1, 128)] public int CompressionThreshold;


    private static void RandomData(Span<byte> input)
    {
        for (int i = 0; i < input.Length; i++)
            input[i] = (byte)(i % 8);
    }

    private Stream _mainStream;

    [GlobalSetup]
    public async Task Setup()
    {
        MemoryStream ms = new MemoryStream();
        _mainStream = ms;
        Random r = new Random(73);
        await using var writer = new MinecraftPacketSender(ms, leaveOpen: true);

        writer.CompressionThreshold = CompressionThreshold;

        var allocator = ArrayPool<byte>.Shared.ToAllocator();


        for (int i = 0; i < PacketsCount; i++)
        {
            var buffer = allocator.AllocateExactly(r.Next(20, 200));
            RandomData(buffer.Span.Slice(5));

            OutputPacket packet = new OutputPacket(buffer);

            await writer.SendAndDisposeAsync(packet, new CancellationToken());
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
    }


    [Benchmark]
    public async Task ReadPacketsStreaming()
    {
        await using var reader = new MinecraftPacketReader(_mainStream);
        reader.CompressionThreshold = CompressionThreshold;

        for (int i = 0; i < PacketsCount; i++)
        {
            _ = await reader.ReadPacketAsync();
        }
    }


    [Benchmark]
    public async Task ReadPacketsWithPipeLines()
    {
        var reader = new MinecraftPacketPipeReader(PipeReader.Create(_mainStream))
        {
            CompressionThreshold = CompressionThreshold
        };
        int count = 0;
        await foreach (var packet in reader.ReadPacketsAsync())
        {
            count++;
            if (count == PacketsCount)
                break;
        }
    }
}