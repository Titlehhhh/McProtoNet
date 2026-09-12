using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Entries");
        writer.WriteStartArray();
        foreach (var item0 in Entries)
        {
            item0.WriteJson(writer);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.set_game_rule", "SetGameRule", PacketPhase.Play, PacketDirection.Serverbound, 50);

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
