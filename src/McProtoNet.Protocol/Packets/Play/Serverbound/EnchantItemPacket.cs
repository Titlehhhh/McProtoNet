using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("EnchantItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x08)]
[PacketId(751, 754, 0x08)]
[PacketId(755, 758, 0x07)]
[PacketId(759, 759, 0x09)]
[PacketId(760, 760, 0x0A)]
[PacketId(761, 761, 0x09)]
[PacketId(762, 763, 0x0A)]
[PacketId(764, 765, 0x0C)]
[PacketId(766, 767, 0x0D)]
[PacketId(768, 770, 0x0F)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x10)]
public sealed partial class EnchantItemPacket : IClientPacket
{
    public sbyte Enchantment { get; set; }
    public VFirst_766Fields? VFirst_766 { get; set; }
    public V767_LastFields? V767_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedByte(Enchantment);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 766:
            {
                var fields = VFirst_766 ?? throw new InvalidOperationException("EnchantItemPacket 0-766 fields missing.");
                writer.WriteSignedByte(fields.WindowId);
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V767_Last ?? throw new InvalidOperationException("EnchantItemPacket 767-last fields missing.");
                writer.WriteUnsignedByte(fields.WindowId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EnchantItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Enchantment = reader.ReadSignedByte();
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 766:
            {
                VFirst_766 = new VFirst_766Fields { WindowId = reader.ReadSignedByte() };
                return;
            }
            case >= 767 and <= MinecraftVersion.LatestProtocol:
            {
                V767_Last = new V767_LastFields { WindowId = reader.ReadUnsignedByte() };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EnchantItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_766Fields { public sbyte WindowId { get; set; } }
    public struct V767_LastFields { public byte WindowId { get; set; } }
}