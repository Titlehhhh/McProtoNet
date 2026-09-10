using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public readonly partial record struct WaypointColor(int Red, int Green, int Blue) : IProtocolType<WaypointColor>
{
    public static WaypointColor Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WaypointColor>(protocolVersion);
        var red = reader.ReadUnsignedByte();
        var green = reader.ReadUnsignedByte();
        var blue = reader.ReadUnsignedByte();
        return new WaypointColor(red, green, blue);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WaypointColor>(protocolVersion);
        writer.WriteUnsignedByte((byte)Red);
        writer.WriteUnsignedByte((byte)Green);
        writer.WriteUnsignedByte((byte)Blue);
    }
}
