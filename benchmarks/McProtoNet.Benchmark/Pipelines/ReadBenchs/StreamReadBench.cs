using System.IO;
using System.Threading.Tasks;
using McProtoNet.Transport.Framing;
namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class StreamReadBench : IReceiveBench
{
    private PacketStreamReader _reader;

    private Stream _stream;

    public StreamReadBench()
    {
    }


    public Task Setup(Stream stream, int compressionThreshold)
    {
        _reader = new PacketStreamReader(stream);
        _reader.CompressionThreshold = compressionThreshold;
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount)
    {
        for (var i = 0; i < packetsCount; i++)
        {
            (await _reader.ReadPacketAsync()).Dispose();
        }
    }

    public async Task Cleanup()
    {
        await _reader.DisposeAsync();
    }
}