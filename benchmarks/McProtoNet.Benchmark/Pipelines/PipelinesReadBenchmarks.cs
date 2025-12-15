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

[Config(typeof(ConfigWithCustomEnvVars))]
[MemoryDiagnoser]
public class PipelinesReadBenchmarks
{
    private class ConfigWithCustomEnvVars : ManualConfig
    {
        public ConfigWithCustomEnvVars()
        {
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0)
                .WithEnvironmentVariables(
                    new EnvironmentVariable("DOTNET_RuntimeAsync", "true")
                )
                .WithToolchain(InProcessNoEmitToolchain.Instance)
                .WithId("RuntimeAsync"));
        }
    }
    
    [Params(1_000_000)] public int PacketsCount;
    [Params(-1)] public int CompressionThreshold;

    [Params(BenchType.BufferedStream, BenchType.Pipelines, BenchType.Pipelines2)]
    public BenchType Bench { get; set; }

    private TestServer _server = new();

    private readonly StreamReadBench _streamBench = new();
    private readonly BufferedStreamReadBench _bufferedStreamBench = new BufferedStreamReadBench();
    private readonly PipelinesReadBench _pipeBench = new PipelinesReadBench();
    private readonly Pipelines2ReadBench _pipe2Bench = new Pipelines2ReadBench();

    private IReceiveBench _activeBench;
    private Stream _stream;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        await _server.Run(PacketsCount, CompressionThreshold, ServerMode.Receive);
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
    public async Task ReadPackets()
    {
        if (_activeBench == null) throw new InvalidOperationException("Active bench is not configured.");
        await _activeBench.Run(PacketsCount);
    }
}