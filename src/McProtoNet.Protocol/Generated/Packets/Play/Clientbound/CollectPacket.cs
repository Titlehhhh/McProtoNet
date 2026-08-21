using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.collect", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("CollectedEntityId", "int")]
[PacketField("CollectorEntityId", "int")]
[PacketField("PickupItemCount", "int")]
public sealed partial record CollectPacket(int CollectedEntityId, int CollectorEntityId, int PickupItemCount) : IPacket<CollectPacket>, IPacket
{
    public static CollectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CollectPacket>(protocolVersion);
        var collectedEntityId = reader.ReadVarInt();
        var collectorEntityId = reader.ReadVarInt();
        var pickupItemCount = reader.ReadVarInt();
        return new CollectPacket(collectedEntityId, collectorEntityId, pickupItemCount);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CollectPacket>(protocolVersion);
        writer.WriteVarInt(CollectedEntityId);
        writer.WriteVarInt(CollectorEntityId);
        writer.WriteVarInt(PickupItemCount);
    }

    public static PacketIdentity Identity => new("play.toClient.collect", "Collect", PacketPhase.Play, PacketDirection.Clientbound, 19);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x55;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x55;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x62;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x65;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x67;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x6A;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x6C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x6F;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x76;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x75;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x7A;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x7C;
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
