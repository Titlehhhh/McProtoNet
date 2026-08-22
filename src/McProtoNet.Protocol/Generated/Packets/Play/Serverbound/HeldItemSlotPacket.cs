using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.held_item_slot", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("SlotId", "int")]
public sealed partial record HeldItemSlotPacket(int SlotId) : IPacket<HeldItemSlotPacket>, IPacket
{
    public static HeldItemSlotPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        var slotId = reader.ReadSignedShort();
        return new HeldItemSlotPacket(slotId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        writer.WriteSignedShort((short)SlotId);
    }

    public static PacketIdentity Identity => new("play.toServer.held_item_slot", "HeldItemSlot", PacketPhase.Play, PacketDirection.Serverbound, 25);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x35;
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
