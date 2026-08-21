using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.player_remove", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Players", "Guid[]")]
public sealed partial record PlayerRemovePacket(Guid[] Players) : IPacket<PlayerRemovePacket>, IPacket
{
    public static PlayerRemovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerRemovePacket>(protocolVersion);
        int playersCount = reader.ReadVarInt();
        var players = new Guid[playersCount];
        for (int i = 0; i < players.Length; i++)
            players[i] = reader.ReadUUID();
        return new PlayerRemovePacket(players);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerRemovePacket>(protocolVersion);
        writer.WriteVarInt(Players.Length);
        foreach (var playersItem in Players)
            writer.WriteUUID(playersItem);
    }

    public static PacketIdentity Identity => new("play.toClient.player_remove", "PlayerRemove", PacketPhase.Play, PacketDirection.Clientbound, 66);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x3D;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x3E;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x45;
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
