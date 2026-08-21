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
using McProtoNet.Primitives;
using McProtoNet.Transport.Framing;
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
        await using var writer = new PacketStreamWriter(ms, leaveOpen: true);

        writer.CompressionThreshold = CompressionThreshold;

        for (int i = 0; i < PacketsCount; i++)
        {
            var buffer = MemoryOwner<byte>.Allocate(r.Next(20, 200));
            RandomData(buffer.Span.Slice(5));

            OutgoingPacket packet = new OutgoingPacket(buffer);

            await writer.WriteAndDisposeAsync(packet, new CancellationToken());
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
    }


    [Benchmark]
    public async Task ReadPacketsOneByOne()
    {
        _mainStream.Position = 0;
        await using var reader = new PacketStreamReader(_mainStream, leaveOpen: true);
        reader.CompressionThreshold = CompressionThreshold;

        for (int i = 0; i < PacketsCount; i++)
        {
            _ = await reader.ReadPacketAsync();
        }
    }


    [Benchmark]
    public async Task ReadPacketsBuffered()
    {
        _mainStream.Position = 0;
        using var reader = new BufferedPacketReader(_mainStream, CompressionThreshold);

        int count = 0;
        while (count < PacketsCount)
        {
            var batch = await reader.ReadBatchAsync();
            if (batch is { Count: 0, IsCompleted: true }) break;
            foreach (var _ in batch) count++;
        }
    }
}