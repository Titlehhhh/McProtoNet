using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using McProtoNet.Benchmark.Pipelines.ReadBenchs;
using McProtoNet.Benchmark.Pipelines.SendBenchs;

namespace McProtoNet.Benchmark.Pipelines;

[Config(typeof(AntiVirusFriendlyConfig))]
[MemoryDiagnoser]
public class PipelinesSendBenchmarks
{
    [Params(1_000_000)] public int PacketsCount;
    [Params(-1)] public int CompressionThreshold;
    [Params(50)] public int PacketSize;

    [Params( BenchType.Pipelines2)]
    public BenchType Bench { get; set; }

    private TestServer _server = new();


    private readonly StreamSendBench _streamBench = new();
    private readonly BufferedStreamSendBench _bufferedStreamBench = new();
    private readonly PipelinesSendBench _pipeBench = new();
    private readonly Pipelines2SendBench _pipe2Bench = new();

    private byte[] _packet;
    private readonly Random _random = new(40);

    private ISendBench _activeBench;
    private Stream _stream;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
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
            case BenchType.Stream:
                _activeBench = _streamBench;
                break;
            case BenchType.BufferedStream:
                _activeBench = _bufferedStreamBench;
                break;
            case BenchType.Pipelines:
                _activeBench = _pipeBench;
                break;
            case BenchType.Pipelines2:
                _activeBench = _pipe2Bench;
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