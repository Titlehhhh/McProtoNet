using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using McProtoNet.Benchmark.Pipelines.SendBenchs;

namespace McProtoNet.Benchmark.Pipelines;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class SendBenchmarksWithQueue
{
    [Params(1_000_000)] public int PacketsCount;
    [Params(-1)] public int CompressionThreshold;
    [Params(50)] public int PacketSize;

    [Params(-1, 1000)] public int QueueSize { get; set; }

    [Params(BenchType.Pipelines2, BenchType.QueueStream, BenchType.QueuePipe)]
    public BenchType Bench { get; set; }

    private TestServer _server = new();

    private readonly Pipelines2SendBench _pipe2Bench = new();
    private QueueStreamSendBench _queueStreamBench;
    private QueuePipeSendBench _queuePipeBench;

    private byte[] _packet;
    private readonly Random _random = new(40);

    private ISendBench _activeBench;
    private Stream _stream;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        _queueStreamBench = new(QueueSize);
        _queuePipeBench = new(QueueSize);
        _packet = new byte[PacketSize];
        _random.NextBytes(_packet);
        await _server.Run(PacketsCount, CompressionThreshold, ServerMode.Send);
    }

    [IterationSetup]
    public async Task IterationSetup()
    {
        _stream = await Connect();

        switch (Bench)
        {
            case BenchType.Pipelines2:
                _activeBench = _pipe2Bench;
                break;
            case BenchType.QueueStream:
                _activeBench = _queueStreamBench;
                break;
            case BenchType.QueuePipe:
                _activeBench = _queuePipeBench;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await _activeBench.Setup(_stream, CompressionThreshold);
    }

    private static async Task<Stream> Connect()
    {
        var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 6060);
        return client.GetStream();
    }

    [IterationCleanup]
    public async Task IterationCleanup()
    {
        if (_activeBench != null)
        {
            await _activeBench.Cleanup();
            _activeBench = null;
        }

        if (_stream != null)
        {
            try
            {
                _stream.Dispose();
            }
            catch
            {
                /* ignore */
            }

            _stream = null;
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _server.Stop();
    }

    [Benchmark]
    public async Task SendPackets()
    {
        if (_activeBench == null) throw new InvalidOperationException("Active bench is not configured.");
        await _activeBench.Run(PacketsCount, _packet);
    }
}