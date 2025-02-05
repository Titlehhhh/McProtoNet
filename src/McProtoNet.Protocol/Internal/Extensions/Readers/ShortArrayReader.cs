using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct ShortArrayReader : IArrayReader<short>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<short> destination)
    {
        reader.ReadArrayInt16BigEndian(destination);
    }
}