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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
