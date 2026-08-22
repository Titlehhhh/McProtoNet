using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.resource_pack_receive", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Result", "int")]
[PacketField("Uuid", "Guid", Group = "V765_Last", From = 765)]
public sealed partial record ResourcePackReceivePacket(int Result, ResourcePackReceivePacket.V765_LastLayer? V765_Last = null) : IPacket<ResourcePackReceivePacket>, IPacket
{
    public readonly record struct V765_LastLayer(Guid Uuid);
    public static ResourcePackReceivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackReceivePacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            var result = reader.ReadVarInt();
            return new ResourcePackReceivePacket(result);
        }

        if (protocolVersion >= 765)
        {
            var uuid = reader.ReadUUID();
            var result = reader.ReadVarInt();
            return new ResourcePackReceivePacket(result, V765_Last: new V765_LastLayer(uuid));
        }

        throw new System.NotSupportedException($"ResourcePackReceivePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ResourcePackReceivePacket>(protocolVersion);
        if (protocolVersion <= 764)
        {
            writer.WriteVarInt(Result);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("ResourcePackReceivePacket", protocolVersion, "V765_Last");
            Guid Uuid = layer.Uuid;
            writer.WriteUUID(Uuid);
            writer.WriteVarInt(Result);
            return;
        }

        throw new System.NotSupportedException($"ResourcePackReceivePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.resource_pack_receive", "ResourcePackReceive", PacketPhase.Play, PacketDirection.Serverbound, 43);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x31;
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
