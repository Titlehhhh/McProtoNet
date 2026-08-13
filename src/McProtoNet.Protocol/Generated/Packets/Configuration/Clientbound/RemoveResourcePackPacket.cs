using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.remove_resource_pack", PacketPhase.Configuration, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("configuration.toClient.remove_resource_pack", "RemoveResourcePack", PacketPhase.Configuration, PacketDirection.Clientbound, 10);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x08;
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
