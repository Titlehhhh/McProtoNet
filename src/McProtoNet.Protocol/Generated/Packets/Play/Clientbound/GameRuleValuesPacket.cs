using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.game_rule_values", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Values", "GameRule[]")]
public sealed partial record GameRuleValuesPacket(GameRule[] Values) : IPacket<GameRuleValuesPacket>, IPacket
{
    public static GameRuleValuesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameRuleValuesPacket>(protocolVersion);
        int valuesCount = reader.ReadVarInt();
        var values = new GameRule[valuesCount];
        for (int i = 0; i < values.Length; i++)
            values[i] = reader.ReadType<GameRule>(protocolVersion);
        return new GameRuleValuesPacket(values);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameRuleValuesPacket>(protocolVersion);
        writer.WriteVarInt(Values.Length);
        foreach (var valuesItem in Values)
            writer.WriteType<GameRule>(valuesItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.game_rule_values", "GameRuleValues", PacketPhase.Play, PacketDirection.Clientbound, 44);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x27;
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
