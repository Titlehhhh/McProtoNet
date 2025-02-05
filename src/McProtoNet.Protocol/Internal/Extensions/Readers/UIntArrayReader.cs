using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct UIntArrayReader : IArrayReader<uint>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<uint> destination)
    {
        reader.ReadArrayUnsignedInt32BigEndian(destination);
    }
}