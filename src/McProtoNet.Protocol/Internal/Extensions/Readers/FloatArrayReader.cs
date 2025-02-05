using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct FloatArrayReader : IArrayReader<float>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<float> destination)
    {
        reader.ReadArrayFloatBigEndian(destination);
    }
}