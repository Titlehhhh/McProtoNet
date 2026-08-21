using System.IO;
using System.Threading.Tasks;
using McProtoNet.Transport.Framing;
namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class BufferedStreamReadBench : IReceiveBench
{
    private PacketStreamReader _reader;

    private Stream _stream;

    public BufferedStreamReadBench()
    {
        
    }


    public Task Setup(Stream stream, int compressionThreshold)
    {
        stream = new BufferedStream(stream);
        _reader = new PacketStreamReader(stream);
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