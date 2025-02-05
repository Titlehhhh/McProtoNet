using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct DoubleArrayReader : IArrayReader<double>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<double> destination)
    {
        reader.ReadArrayDoubleBigEndian(destination);
    }
}