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

[MemoryDiagnoser]
public class ReadPacketBenchmarks
{
    [Params(1_000_000)] public int PacketsCount = 1_000_000;

    [Params(-1, 128)] public int CompressionThreshold = 1_000_000;

    [GlobalSetup]
    public async Task Setup()
    {
        Random r = new Random(73);
        var writer = new MinecraftPacketSender();

        writer.SwitchCompression(CompressionThreshold);

        var allocator = ArrayPool<byte>.Shared.ToAllocator();
        using (var fs = File.OpenWrite("data.bin"))
        {
            writer.BaseStream = fs;

            for (int i = 0; i < PacketsCount; i++)
            {
                var buffer = allocator.AllocateExactly(r.Next(20, 200));
                r.NextBytes(buffer.Span.Slice(5));

                OutputPacket packet = new OutputPacket(buffer);

                await writer.SendAndDisposeAsync(packet, new CancellationToken());
            }
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
    }

    [Benchmark]
    public async Task ReadPackets()
    {
        await using var fs = File.OpenRead("data.bin");
        var reader = new MinecraftPacketReader();
        reader.EnableCompression(CompressionThreshold);
        reader.BaseStream = fs;
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
        await using var fs = File.OpenRead("data.bin");

        var reader = new MinecraftPacketPipeReader(PipeReader.Create(fs));
        reader.CompressionThreshold = CompressionThreshold;
        for (int i = 0; i < PacketsCount; i++)
        {
            var inputPacket = await reader.ReadPacketAsync();
            
            
            inputPacket.Dispose();
        }
    }


    public static async ValueTask<int> ReadVarIntAsync(Stream stream, CancellationToken token = default)
    {
        var buff = ArrayPool<byte>.Shared.Rent(1);
        Memory<byte> memory = buff.AsMemory(0, 1);
        try
        {
            var numRead = 0;
            var result = 0;
            byte read;
            do
            {
                await stream.ReadExactlyAsync(memory, token);

                read = buff[0];


                var value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5) throw new InvalidOperationException("VarInt is too big");
            } while ((read & 0b10000000) != 0);

            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buff);
        }
    }


    public static ValueTask WriteVarIntAsync(Stream stream, int value, CancellationToken token = default)
    {
        var unsigned = (uint)value;


        var data = ArrayPool<byte>.Shared.Rent(5);
        try
        {
            var len = 0;
            do
            {
                token.ThrowIfCancellationRequested();
                var temp = (byte)(unsigned & 127);
                unsigned >>= 7;

                if (unsigned != 0)
                    temp |= 128;
                data[len++] = temp;
            } while (unsigned != 0);

            return stream.WriteAsync(data.AsMemory(0, len), token);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(data);
        }
    }
}