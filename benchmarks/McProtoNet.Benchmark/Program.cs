using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using CommandLine;

namespace McProtoNet.Benchmark;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "memory-bench")
        {
            MemoryBench.Run(args[1..]);
            return;
        }

        if (args.Length > 0 && args[0] == "alloc-bench")
        {
            AllocatorBench.Run();
            return;
        }

        var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}