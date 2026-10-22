using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace McProtoNet.Benchmark.Pipelines;

public class ShortRunConfig : ManualConfig
{
    public ShortRunConfig()
    {
        AddJob(Job.ShortRun.WithToolchain(InProcessNoEmitToolchain.Instance));
    }
}
