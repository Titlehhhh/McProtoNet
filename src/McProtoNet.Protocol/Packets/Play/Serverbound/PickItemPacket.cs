using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("PickItem", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 768)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x18)]
[PacketId(751, 754, 0x18)]
[PacketId(755, 758, 0x17)]
[PacketId(759, 759, 0x19)]
[PacketId(760, 760, 0x1A)]
[PacketId(761, 761, 0x19)]
[PacketId(762, 763, 0x1A)]
[PacketId(764, 764, 0x1C)]
[PacketId(765, 765, 0x1D)]
[PacketId(766, 767, 0x20)]
[PacketId(768, 768, 0x22)]
public sealed partial class PickItemPacket : IClientPacket
{
    public int Slot { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 768:
                writer.WriteVarInt(Slot);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PickItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 768:
                Slot = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PickItemPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}