using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class WaypointIcon : IProtocolType<WaypointIcon>
{
    public string Style { get; }
    public WaypointColor? Color { get; }

    public WaypointIcon(string style, WaypointColor? color)
    {
        Style = style;
        Color = color;
    }

    public static WaypointIcon Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WaypointIcon>(protocolVersion);
        var style = reader.ReadString();
        WaypointColor? color = null;
        if (reader.ReadBoolean())
            color = reader.ReadType<WaypointColor>(protocolVersion);
        return new WaypointIcon(style, color);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WaypointIcon>(protocolVersion);
        writer.WriteString(Style);
        writer.WriteBoolean(Color is not null);
        if (Color is { } colorValue)
            writer.WriteType<WaypointColor>(colorValue, protocolVersion);
    }
}
