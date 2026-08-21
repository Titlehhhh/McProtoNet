using System;
using System.IO;
using System.Threading.Tasks;
using McProtoNet.Transport;
namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class NextApiReadBench : IReceiveBench
{
    private MinecraftConnection _connection;

    private Stream _stream;

    public Task Setup(Stream stream, int compressionThreshold)
    {
        _stream = stream;
        _connection = MinecraftConnection.Create(stream);
        _connection.CompressionThreshold = compressionThreshold;
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount)
    {
        var count = 0;
        await foreach (var packet in _connection.ReadPacketsAsync())
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
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        _stream = null;
    }
}
