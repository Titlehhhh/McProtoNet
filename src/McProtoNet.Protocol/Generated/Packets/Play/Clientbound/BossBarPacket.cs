using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.boss_bar", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityUuid", "Guid")]
[PacketField("Action", "BossBarAction")]
public sealed partial record BossBarPacket(Guid EntityUuid, BossBarAction Action) : IPacket<BossBarPacket>, IPacket
{
    public static BossBarPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BossBarPacket>(protocolVersion);
        var entityUuid = reader.ReadUUID();
        var _action = reader.ReadVarInt();
        var action = BossBarAction.Read(ref reader, protocolVersion, (int)_action);
        return new BossBarPacket(entityUuid, action);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BossBarPacket>(protocolVersion);
        writer.WriteUUID(EntityUuid);
        writer.WriteVarInt(Action.Discriminator(protocolVersion));
        Action.Write(writer, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.boss_bar", "BossBar", PacketPhase.Play, PacketDirection.Clientbound, 9);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x09;
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
