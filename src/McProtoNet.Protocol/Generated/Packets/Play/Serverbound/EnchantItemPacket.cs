using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.enchant_item", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("WindowId", "int")]
[PacketField("Enchantment", "int")]
public sealed partial record EnchantItemPacket(int WindowId, int Enchantment) : IPacket<EnchantItemPacket>, IPacket
{
    public static EnchantItemPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EnchantItemPacket>(protocolVersion);
        if (protocolVersion <= 766)
        {
            var windowId = reader.ReadSignedByte();
            var enchantment = reader.ReadSignedByte();
            return new EnchantItemPacket(windowId, enchantment);
        }

        if (protocolVersion >= 767 && protocolVersion <= 767)
        {
            var windowId = reader.ReadUnsignedByte();
            var enchantment = reader.ReadVarInt();
            return new EnchantItemPacket(windowId, enchantment);
        }

        if (protocolVersion >= 768)
        {
            var windowId = reader.ReadVarInt();
            var enchantment = reader.ReadVarInt();
            return new EnchantItemPacket(windowId, enchantment);
        }

        throw new System.NotSupportedException($"EnchantItemPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EnchantItemPacket>(protocolVersion);
        if (protocolVersion <= 766)
        {
            writer.WriteSignedByte((sbyte)WindowId);
            writer.WriteSignedByte((sbyte)Enchantment);
            return;
        }

        if (protocolVersion >= 767 && protocolVersion <= 767)
        {
            writer.WriteUnsignedByte((byte)WindowId);
            writer.WriteVarInt(Enchantment);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(WindowId);
            writer.WriteVarInt(Enchantment);
            return;
        }

        throw new System.NotSupportedException($"EnchantItemPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.enchant_item", "EnchantItem", PacketPhase.Play, PacketDirection.Serverbound, 20);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x11;
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
