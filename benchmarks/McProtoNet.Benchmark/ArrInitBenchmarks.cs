using BenchmarkDotNet.Attributes;

namespace McProtoNet.Benchmark;

[MemoryDiagnoser(displayGenColumns: true)]
public class ArrInitBenchmarks
{
    [Benchmark]
    public void A()
    {
        for (int i = 0; i < 100; i++)
        {
            B([0,0,0,0]);
        }
    }

    public void B(int[] arr)
    {
        int a = arr[0];
        int c = a + 1;
    }
}