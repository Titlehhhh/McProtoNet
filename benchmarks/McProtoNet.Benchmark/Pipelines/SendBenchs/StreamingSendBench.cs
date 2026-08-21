using System;
using System.IO;
using System.Threading.Tasks;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Benchmark.Pipelines.SendBenchs;

public class StreamingSendBench : ISendBench
{
    private BufferedPacketWriter _writer;

    public Task Setup(Stream stream, int compressionThreshold)
    {
        _writer = new BufferedPacketWriter(stream, compressionThreshold);
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount, ReadOnlyMemory<byte> packet)
    {
        for (var i = 0; i < packetsCount; i++)
        {
            _writer.WritePacket(packet.Span);
            await _writer.FlushAsync();
        }
    }

    public Task Cleanup()
    {
        _writer?.Dispose();
        _writer = null;
        return Task.CompletedTask;
    }
}
