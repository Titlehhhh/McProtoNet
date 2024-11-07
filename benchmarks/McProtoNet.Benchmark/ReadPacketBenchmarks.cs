using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
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

    private Stream ms;

    private static void RandomData(Span<byte> input)
    {
        for (int i = 0; i < input.Length; i++)
            input[i] = (byte)(i % 8);
    }

    [GlobalSetup]
    public async Task Setup()
    {
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
        ms.Dispose();
    }

    [Benchmark]
    public async Task ReadPacketsStandart()
    {
       
        ms.Position = 0;

        var reader = new MinecraftPacketReader();
        reader.EnableCompression(CompressionThreshold);
        reader.BaseStream = ms;
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
       
        ms.Position = 0;
        
        var reader = new MinecraftPacketPipeReader(PipeReader.Create(ms));
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