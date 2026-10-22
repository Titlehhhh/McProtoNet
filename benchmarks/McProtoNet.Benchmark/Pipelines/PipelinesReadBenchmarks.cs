using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using McProtoNet.Benchmark.Pipelines.ReadBenchs;

namespace McProtoNet.Benchmark.Pipelines;

[Config(typeof(PacedSocketConfig))]
[MemoryDiagnoser]
public class PipelinesReadBenchmarks
{
    private class PacedSocketConfig : ManualConfig
    {
        public PacedSocketConfig()
        {
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0)
                .WithToolchain(InProcessNoEmitToolchain.Instance)
                .WithInvocationCount(1)
                .WithUnrollFactor(1)
                .WithWarmupCount(2)
                .WithIterationCount(6)
                .WithId("PacedSocket"));
        }
    }

    [Params(100_000)] public int PacketsCount;
    [Params(-1, 0)] public int CompressionThreshold;
    [Params(1, 100)] public int Connections;
    [Params(0, 10)] public int GapMicroseconds;

    [Params(BenchType.Stream, BenchType.BufferedStream, BenchType.Streaming)]
    public BenchType Bench { get; set; }

    private readonly TestServer _server = new();

    private TcpClient[] _clients = [];
    private Stream[] _streams = [];
    private IReceiveBench[] _benches = [];

    private int PerConnection => PacketsCount / Connections;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        await _server.Run(PacketsCount, CompressionThreshold, ServerMode.Receive,
            TimeSpan.FromMicroseconds(GapMicroseconds), PerConnection);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _clients = new TcpClient[Connections];
        _streams = new Stream[Connections];
        _benches = new IReceiveBench[Connections];

        for (var i = 0; i < Connections; i++)
        {
            var client = new TcpClient { NoDelay = true, LingerState = new LingerOption(true, 0) };
            client.Connect("127.0.0.1", 6060);

            _clients[i] = client;
            _streams[i] = client.GetStream();
            _benches[i] = Create();
            _benches[i].Setup(_streams[i], CompressionThreshold).GetAwaiter().GetResult();
        }
    }

    private IReceiveBench Create()
    {
        return Bench switch
        {
            BenchType.Stream => new StreamReadBench(),
            BenchType.BufferedStream => new BufferedStreamReadBench(),
            BenchType.Streaming => new StreamingReadBench(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        foreach (var bench in _benches)
        {
            try
            {
                bench?.Cleanup().GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        foreach (var stream in _streams)
        {
            try
            {
                stream?.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        foreach (var client in _clients)
        {
            try
            {
                client?.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        _benches = [];
        _streams = [];
        _clients = [];
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _server.Stop();
    }

    [Benchmark]
    public Task ReadPackets()
    {
        var per = PerConnection;
        if (_benches.Length == 1) return _benches[0].Run(per);

        var readers = new Task[_benches.Length];
        for (var i = 0; i < _benches.Length; i++)
        {
            var bench = _benches[i];
            readers[i] = Task.Run(() => bench.Run(per));
        }

        return Task.WhenAll(readers);
    }
}
