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

    public static PacketIdentity Identity => new("play.toClient.remove_resource_pack", "RemoveResourcePack", PacketPhase.Play, PacketDirection.Clientbound, 73);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x43;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x45;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x4E;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x50;
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
