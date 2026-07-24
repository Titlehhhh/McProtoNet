using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Vec2f(float X, float Y) : IProtocolType<Vec2f>
{
    public static Vec2f Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Vec2f>(protocolVersion);
        var x = reader.ReadFloat();
        var y = reader.ReadFloat();
        return new Vec2f(x, y);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Vec2f>(protocolVersion);
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
    }
}
