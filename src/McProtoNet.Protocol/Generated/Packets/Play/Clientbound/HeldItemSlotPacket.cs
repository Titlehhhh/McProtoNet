using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.held_item_slot", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Slot", "int")]
public sealed partial record HeldItemSlotPacket(int Slot) : IPacket<HeldItemSlotPacket>, IPacket
{
    public static HeldItemSlotPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            var slot = reader.ReadSignedByte();
            return new HeldItemSlotPacket(slot);
        }

        if (protocolVersion >= 769)
        {
            var slot = reader.ReadVarInt();
            return new HeldItemSlotPacket(slot);
        }

        throw new System.NotSupportedException($"HeldItemSlotPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            writer.WriteSignedByte((sbyte)Slot);
            return;
        }

        if (protocolVersion >= 769)
        {
            writer.WriteVarInt(Slot);
            return;
        }

        throw new System.NotSupportedException($"HeldItemSlotPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.held_item_slot", "HeldItemSlot", PacketPhase.Play, PacketDirection.Clientbound, 45);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x48;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x47;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x4D;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x4F;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x51;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x63;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x62;
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
