using System;
using System.IO;
using System.Threading.Tasks;
using McProtoNet.Next;

namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class NextApiReadBench : IReceiveBench
{
    private MinecraftClient _client;

    private Stream _stream;

    public Task Setup(Stream stream, int compressionThreshold)
    {
        _stream = stream;
        _client = MinecraftClient.Create(stream, new MinecraftClientOptions
        {
            CompressionThreshold = compressionThreshold
        });
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount)
    {
        var count = 0;
        await foreach (var packet in _client.ReadPacketsAsync())
        {
            count++;
            if (count == packetsCount)
                break;
        }

        if (count != packetsCount)
        {
            Environment.FailFast($"Packets count mismatch {count} != {packetsCount}");
        }
    }

    public async Task Cleanup()
    {
        if (_client is not null)
        {
            await _client.DisposeAsync();
            _client = null;
        }

        _stream = null;
    }
}
