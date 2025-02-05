using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct ByteArrayReader : IArrayReader<byte>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<byte> destination)
    {
        reader.Read(destination);
    }
}