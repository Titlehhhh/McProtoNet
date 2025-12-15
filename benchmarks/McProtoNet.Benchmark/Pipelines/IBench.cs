using System;
using System.IO;
using System.Threading.Tasks;

namespace McProtoNet.Benchmark.Pipelines;

public interface IBench 
{
    Task Setup(Stream stream, int compressionThreshold);
    
    
    Task Cleanup();
}

public interface ISendBench : IBench
{
    Task Run(int packetsCount, ReadOnlyMemory<byte> packet);
}

public interface IReceiveBench : IBench
{
    Task Run(int packetsCount);
}