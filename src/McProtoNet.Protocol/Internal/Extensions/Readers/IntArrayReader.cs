using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct IntArrayReader : IArrayReader<int>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<int> destination)
    {
        reader.ReadArrayInt32BigEndian(destination);
    }
}