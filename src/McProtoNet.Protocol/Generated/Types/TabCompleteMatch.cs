using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class TabCompleteMatch : IProtocolType<TabCompleteMatch>
{
    public string Match { get; }
    public string? TooltipJson { get; }
    public NbtTag? Tooltip { get; }

    public TabCompleteMatch(string match, string? tooltipJson, NbtTag? tooltip)
    {
        Match = match;
        TooltipJson = tooltipJson;
        Tooltip = tooltip;
    }

    public static TabCompleteMatch Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompleteMatch>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var match = reader.ReadString();
            string? tooltipJson = null;
            if (reader.ReadBoolean())
                tooltipJson = reader.ReadString();
            return new TabCompleteMatch(match, tooltipJson, default!);
        }

        if (protocolVersion >= 765)
        {
            var match = reader.ReadString();
            NbtTag? tooltip = null;
            if (reader.ReadBoolean())
                tooltip = reader.ReadNbtTag(false)!;
            return new TabCompleteMatch(match, default!, tooltip);
        }

        throw new System.NotSupportedException($"TabCompleteMatch has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompleteMatch>(protocolVersion);
        if (protocolVersion <= 764)
        {
            writer.WriteString(Match);
            writer.WriteBoolean(TooltipJson is not null);
            if (TooltipJson is { } tooltipJsonValue)
                writer.WriteString(tooltipJsonValue);
            return;
        }

        if (protocolVersion >= 765)
        {
            writer.WriteString(Match);
            writer.WriteBoolean(Tooltip is not null);
            if (Tooltip is { } tooltipValue)
                writer.WriteNbt(tooltipValue);
            return;
        }

        throw new System.NotSupportedException($"TabCompleteMatch has no wire layout for protocol version {protocolVersion}.");
    }
}
