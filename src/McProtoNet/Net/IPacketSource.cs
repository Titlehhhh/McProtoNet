using System.Buffers;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

public interface IPacketSource
{
    int GetId(int token);
    ReadOnlySequence<byte> GetData(int token);
}