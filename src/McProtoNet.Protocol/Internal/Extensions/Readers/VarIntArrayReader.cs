using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct VarIntArrayReader : IArrayReader<int>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<int> destination)
    {
        for (int i = 0; i < destination.Length; i++)
        {
            destination[i] = reader.ReadVarInt();
        }
    }
}