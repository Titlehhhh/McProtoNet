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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
