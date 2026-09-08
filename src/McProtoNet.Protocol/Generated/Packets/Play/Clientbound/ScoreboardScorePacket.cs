using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.scoreboard_score", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityName", "string")]
[PacketField("ObjectiveName", "string")]
[PacketField("Value", "int?")]
[PacketField("Action", "int", Group = "VUntil764", To = 764)]
[PacketField("DisplayName", "NbtTag?", Group = "V765_Last", From = 765)]
[PacketField("NumberFormat", "int?", Group = "V765_Last", From = 765)]
[PacketField("Styling", "NbtTag?", Group = "V765_Last", From = 765)]
public sealed partial record ScoreboardScorePacket(string EntityName, string ObjectiveName, int? Value, ScoreboardScorePacket.VUntil764Layer? VUntil764 = null, ScoreboardScorePacket.V765_LastLayer? V765_Last = null) : IPacket<ScoreboardScorePacket>, IPacket
{
    public readonly record struct VUntil764Layer(int Action);
    public readonly record struct V765_LastLayer(NbtTag? DisplayName, int? NumberFormat, NbtTag? Styling);
    public static ScoreboardScorePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ScoreboardScorePacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var entityName = reader.ReadString();
            var action = reader.ReadVarInt();
            var objectiveName = reader.ReadString();
            int? value = default;
            if (action == 0)
            {
                var valueValue = reader.ReadVarInt();
                value = valueValue;
            }

            return new ScoreboardScorePacket(entityName, objectiveName, value, VUntil764: new VUntil764Layer(action));
        }

        if (protocolVersion >= 765)
        {
            var entityName = reader.ReadString();
            var objectiveName = reader.ReadString();
            var value = reader.ReadVarInt();
            NbtTag? displayName = null;
            if (reader.ReadBoolean())
                displayName = reader.ReadNbtTag(false)!;
            int? numberFormat = null;
            if (reader.ReadBoolean())
                numberFormat = reader.ReadVarInt();
            NbtTag? styling = default;
            if (numberFormat == 1 || numberFormat == 2)
            {
                var stylingValue = reader.ReadNbtTag(false)!;
                styling = stylingValue;
            }

            return new ScoreboardScorePacket(entityName, objectiveName, value, V765_Last: new V765_LastLayer(displayName, numberFormat, styling));
        }

        throw new System.NotSupportedException($"ScoreboardScorePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ScoreboardScorePacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var layer = VUntil764 ?? throw new WrongLayerException("ScoreboardScorePacket", protocolVersion, "VUntil764");
            int Action = layer.Action;
            writer.WriteString(EntityName);
            writer.WriteVarInt(Action);
            writer.WriteString(ObjectiveName);
            if (Action == 0)
            {
                writer.WriteVarInt((Value ?? throw new System.InvalidOperationException("Value is required at this protocol version.")));
            }
            else if (Value is not null)
            {
                throw new System.InvalidOperationException("Value is set, but 'action' does not select it at this protocol version.");
            }

            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("ScoreboardScorePacket", protocolVersion, "V765_Last");
            NbtTag? DisplayName = layer.DisplayName;
            int? NumberFormat = layer.NumberFormat;
            NbtTag? Styling = layer.Styling;
            writer.WriteString(EntityName);
            writer.WriteString(ObjectiveName);
            writer.WriteVarInt((Value ?? throw new System.InvalidOperationException("Value is required at this protocol version.")));
            writer.WriteBoolean(DisplayName is not null);
            if (DisplayName is { } displayNameValue)
                writer.WriteNbt(displayNameValue);
            writer.WriteBoolean(NumberFormat is not null);
            if (NumberFormat is { } numberFormatValue)
                writer.WriteVarInt(numberFormatValue);
            if (NumberFormat == 1 || NumberFormat == 2)
            {
                writer.WriteNbt((Styling ?? throw new System.InvalidOperationException("Styling is required at this protocol version.")));
            }
            else if (Styling is not null)
            {
                throw new System.InvalidOperationException("Styling is set, but 'number_format' does not select it at this protocol version.");
            }

            return;
        }

        throw new System.NotSupportedException($"ScoreboardScorePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.scoreboard_score", "ScoreboardScore", PacketPhase.Play, PacketDirection.Clientbound, 85);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x4D;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x4D;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 759)
        {
            id = 0x56;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x59;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x57;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x5D;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x5F;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x68;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x67;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x6C;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x6E;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
