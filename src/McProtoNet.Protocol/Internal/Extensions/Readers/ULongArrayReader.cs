using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct ULongArrayReader : IArrayReader<ulong>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<ulong> destination)
    {
        reader.ReadArrayUnsignedInt64BigEndian(destination);
    }
}