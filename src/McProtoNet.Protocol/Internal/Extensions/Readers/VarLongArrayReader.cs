using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct VarLongArrayReader : IArrayReader<long>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<long> destination)
    {
        for (int i = 0; i < destination.Length; i++)
        {
            destination[i] = reader.ReadVarLong();
        }
    }
}