using System.Text;
using BenchmarkDotNet.Attributes;

namespace McProtoNet.Benchmark;

[MemoryDiagnoser]
public class StringBenchmarks
{
    private static readonly string Test = "asdasdasdasdsadasasdsadsadasdadsddadsaолрывапоывро аыгна ынае ы нае8фыа7";

    [Benchmark]
    public int Scenario1()
    {
        return 1;
        //return Encoding.UTF8.GetBytes(Test,);
    }
}