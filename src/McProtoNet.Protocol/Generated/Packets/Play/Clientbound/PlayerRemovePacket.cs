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

    public static PacketIdentity Identity => new("play.toClient.player_remove", "PlayerRemove", PacketPhase.Play, PacketDirection.Clientbound, 72);

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
