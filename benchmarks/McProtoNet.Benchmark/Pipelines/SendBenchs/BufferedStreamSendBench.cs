using System;
using System.IO;
using System.Threading.Tasks;
using DotNext.IO;
using McProtoNet.Net;

namespace McProtoNet.Benchmark.Pipelines.SendBenchs;

public class BufferedStreamSendBench : ISendBench
{
    private MinecraftPacketSender _sender;
   

    public Task Setup(Stream stream, int compressionThreshold)
    {
        stream = new PoolingBufferedStream(stream);
        _sender = new MinecraftPacketSender(stream)
        {
            CompressionThreshold = compressionThreshold
        };
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount, ReadOnlyMemory<byte> packet)
    {
        for (int i = 0; i < packetsCount; i++)
        {
            await _sender.SendPacketAsync(packet);
        }
    }

    public async Task Cleanup()
    {
        await _sender.DisposeAsync();
    }
}