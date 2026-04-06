using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Abilities", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1A)]
[PacketId(751, 754, 0x1A)]
[PacketId(755, 758, 0x19)]
[PacketId(759, 759, 0x1B)]
[PacketId(760, 760, 0x1C)]
[PacketId(761, 761, 0x1B)]
[PacketId(762, 763, 0x1C)]
[PacketId(764, 764, 0x1F)]
[PacketId(765, 765, 0x20)]
[PacketId(766, 767, 0x23)]
[PacketId(768, 768, 0x25)]
[PacketId(769, 770, 0x26)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x27)]
public sealed partial class AbilitiesPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteSignedByte(Flags);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AbilitiesPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Flags = reader.ReadSignedByte();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AbilitiesPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public sbyte Flags { get; set; }
}