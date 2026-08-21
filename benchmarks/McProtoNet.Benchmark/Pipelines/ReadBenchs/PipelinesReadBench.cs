using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using McProtoNet.Transport.Pipelines;
namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class PipelinesReadBench : IReceiveBench
{
    private MinecraftPacketPipeReader _reader;


    private Stream _stream;

    public Task Setup(Stream stream, int compressionThreshold)
    {
        _stream = stream;
        _reader = new MinecraftPacketPipeReader(PipeReader.Create(_stream))
        {
            CompressionThreshold = compressionThreshold
        };
        return Task.CompletedTask;
        
    }

    public async Task Run(int packetsCount)
    {
        var count = 0;
        try
        {
            await foreach (var packet in _reader.ReadPacketsAsync())
            {
                count++;
            }
        }
        catch (Exception)
        {
            // ignored
        }

        if (count != packetsCount)
        {
            Environment.FailFast($"Packets count mismatch {count} != {packetsCount}");
        }
    }

    public Task Cleanup()
    {
        _stream?.Dispose();
        return Task.CompletedTask;
    }
}