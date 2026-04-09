using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("LockDifficulty", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x11)]
[PacketId(751, 754, 0x11)]
[PacketId(755, 758, 0x10)]
[PacketId(759, 759, 0x12)]
[PacketId(760, 760, 0x13)]
[PacketId(761, 761, 0x12)]
[PacketId(762, 763, 0x13)]
[PacketId(764, 764, 0x15)]
[PacketId(765, 765, 0x16)]
[PacketId(766, 767, 0x19)]
[PacketId(768, 770, 0x1B)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x1C)]
public sealed partial class LockDifficultyPacket : IClientPacket
{
    public bool Locked { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteBoolean(Locked);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(LockDifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Locked = reader.ReadBoolean();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(LockDifficultyPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}