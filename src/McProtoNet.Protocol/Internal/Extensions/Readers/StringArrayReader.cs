using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public readonly struct StringArrayReader : IArrayReader<string>
{
    public static void Read(ref MinecraftPrimitiveReader reader, int protocolVersion, Span<string> destination)
    {
        for (int i = 0; i < destination.Length; i++)
        {
            destination[i] = reader.ReadString();
        }
    }
}