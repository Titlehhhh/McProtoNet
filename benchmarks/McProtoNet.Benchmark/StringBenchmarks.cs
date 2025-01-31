using System.Text;
using BenchmarkDotNet.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Benchmark;

[MemoryDiagnoser]
public class StringBenchmarks
{
    private static readonly string Test =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";


    [Benchmark]
    public void Scenario1()
    {
        MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();
        for (int i = 0; i < 10; i++)
        {
            writer.WriteStringOld(Test);
        }
    }

    [Benchmark]
    public void Scenario2()
    {
        MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();
        for (int i = 0; i < 10; i++)
        {
            writer.WriteString(Test);
        }
    }
}