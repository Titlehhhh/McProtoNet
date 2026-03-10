using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("ArmAnimation", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2B)]
[PacketId(751, 758, 0x2C)]
[PacketId(759, 759, 0x2E)]
[PacketId(760, 763, 0x2F)]
[PacketId(764, 764, 0x32)]
[PacketId(765, 765, 0x33)]
[PacketId(766, 767, 0x36)]
[PacketId(768, 768, 0x38)]
[PacketId(769, 769, 0x3A)]
[PacketId(770, 770, 0x3B)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x3C)]
public sealed partial class ArmAnimationPacket : IClientPacket
{
    public int Hand { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Hand);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ArmAnimationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Hand = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ArmAnimationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}