using System.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Net;

public readonly struct InputPacket
{
    public readonly int Id;
    public readonly ReadOnlySequence<byte> Data;

    public InputPacket(int id, ReadOnlySequence<byte> data)
    {
        Id = id;
        Data = data;
    }

    public long FullLength => Id.GetVarIntLength() + Data.Length;
}
