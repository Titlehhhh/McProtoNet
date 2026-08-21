using System;
using System.IO;
using System.Threading.Tasks;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Benchmark.Pipelines.ReadBenchs;

public class StreamingReadBench : IReceiveBench
{
    private BufferedPacketReader _reader;

    public Task Setup(Stream stream, int compressionThreshold)
    {
        _reader = new BufferedPacketReader(stream, compressionThreshold);
        return Task.CompletedTask;
    }

    public async Task Run(int packetsCount)
    {
        var count = 0;
        while (count < packetsCount)
        {
            var batch = await _reader.ReadBatchAsync();
            if (batch is { Count: 0, IsCompleted: true })
                Environment.FailFast($"Packets count mismatch {count} != {packetsCount}");

            foreach (var _ in batch) count++;
        }
    }

    public Task Cleanup()
    {
        _reader?.Dispose();
        _reader = null;
        return Task.CompletedTask;
    }
}
