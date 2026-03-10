using System.IO;
using System.Threading.Tasks;
using McProtoNet.Net;

namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class StreamReadBench : IReceiveBench
{
    private MinecraftPacketReader _reader;

    private Stream _stream;

    public StreamReadBench()
    {
    }


    public Task Setup(Stream stream, int compressionThreshold)
    {
        _reader = new MinecraftPacketReader(stream);
        _reader.CompressionThreshold = compressionThreshold;
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount)
    {
        for (var i = 0; i < packetsCount; i++)
        {
            _ = await _reader.ReadPacketAsync();
        }
    }

    public async Task Cleanup()
    {
        await _reader.DisposeAsync();
    }
}