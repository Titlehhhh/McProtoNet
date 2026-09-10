using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toServer.enchant_item", "EnchantItem", PacketPhase.Play, PacketDirection.Serverbound, 24);

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
