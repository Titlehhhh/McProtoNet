using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.set_game_rule", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Entries", "GameRule[]")]
public sealed partial record SetGameRulePacket(GameRule[] Entries) : IPacket<SetGameRulePacket>, IPacket
{
    public static SetGameRulePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetGameRulePacket>(protocolVersion);
        int entriesCount = reader.ReadVarInt();
        var entries = new GameRule[entriesCount];
        for (int i = 0; i < entries.Length; i++)
            entries[i] = reader.ReadType<GameRule>(protocolVersion);
        return new SetGameRulePacket(entries);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetGameRulePacket>(protocolVersion);
        writer.WriteVarInt(Entries.Length);
        foreach (var entriesItem in Entries)
            writer.WriteType<GameRule>(entriesItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.set_game_rule", "SetGameRule", PacketPhase.Play, PacketDirection.Serverbound, 47);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x39;
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
