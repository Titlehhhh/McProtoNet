using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct UShortArrayReader : IArrayReader<ushort>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<ushort> destination)
    {
        reader.ReadArrayUnsignedInt16BigEndian(destination);
    }
}