using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using McProtoNet.Net;

namespace McProtoNet.Benchmark.Pipelines.SendBenchs;

public class PipelinesSendBench : ISendBench
{
    private MinecraftPacketPipeWriter _writer;


    private Stream _stream;

    public Task Setup(Stream stream, int compressionThreshold)
    {
        _stream = stream;
        _writer = new MinecraftPacketPipeWriter(PipeWriter.Create(_stream))
        {
            CompressionThreshold = compressionThreshold
        };
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount, ReadOnlyMemory<byte> packet)
    {
        for (int i = 0; i < packetsCount; i++)
        {
            _writer.WritePacket(packet.Span);
            await _writer.FlushAsync();
        }
    }

    public Task Cleanup()
    {
        _stream?.Dispose();
        return Task.CompletedTask;
    }
}