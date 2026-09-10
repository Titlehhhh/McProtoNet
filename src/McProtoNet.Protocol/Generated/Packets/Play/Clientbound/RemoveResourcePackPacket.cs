using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.remove_resource_pack", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Uuid", "Guid?")]
public sealed partial record RemoveResourcePackPacket(Guid? Uuid) : IPacket<RemoveResourcePackPacket>, IPacket
{
    public static RemoveResourcePackPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RemoveResourcePackPacket>(protocolVersion);
        Guid? uuid = null;
        if (reader.ReadBoolean())
            uuid = reader.ReadUUID();
        return new RemoveResourcePackPacket(uuid);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RemoveResourcePackPacket>(protocolVersion);
        writer.WriteBoolean(Uuid is not null);
        if (Uuid is { } uuidValue)
            writer.WriteUUID(uuidValue);
    }

    public static PacketIdentity Identity => new("play.toClient.remove_resource_pack", "RemoveResourcePack", PacketPhase.Play, PacketDirection.Clientbound, 80);

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
