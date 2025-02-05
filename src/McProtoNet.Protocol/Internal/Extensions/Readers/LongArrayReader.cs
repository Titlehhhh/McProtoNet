using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct LongArrayReader : IArrayReader<long>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<long> destination)
    {
        reader.ReadArrayInt64BigEndian(destination);
    }
}