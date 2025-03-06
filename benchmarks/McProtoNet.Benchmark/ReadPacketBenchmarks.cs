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
        var writer = new MinecraftPacketSender();

        writer.SwitchCompression(CompressionThreshold);

        var allocator = ArrayPool<byte>.Shared.ToAllocator();

        writer.BaseStream = ms;

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
        var reader = new MinecraftPacketReader();
        reader.SwitchCompression(CompressionThreshold);
        reader.BaseStream = _mainStream;
        for (int i = 0; i < PacketsCount; i++)
        {
            InputPacket packet = await reader.ReadNextPacketAsync();
            try
            {
            }
            finally
            {
                packet.Dispose();
            }
        }
    }


    [Benchmark]
    public async Task ReadPacketsWithPipeLines()
    {
        var reader = new MinecraftPacketPipeReader(PipeReader.Create(_mainStream));
        reader.CompressionThreshold = CompressionThreshold;
        int count = 0;
        await foreach (var packet in reader.ReadPacketsAsync())
        {
            packet.Dispose();
            count++;
            if (count == PacketsCount)
                break;
        }
    }
}