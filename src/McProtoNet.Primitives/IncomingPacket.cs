using System.Buffers;
namespace McProtoNet.Primitives;

public readonly struct IncomingPacket
{
    public readonly int Id;
    public readonly ReadOnlySequence<byte> Data;

    public IncomingPacket(int id, ReadOnlySequence<byte> data)
    {
        Id = id;
        Data = data;
    }

    public long FullLength => Id.GetVarIntLength() + Data.Length;
}
