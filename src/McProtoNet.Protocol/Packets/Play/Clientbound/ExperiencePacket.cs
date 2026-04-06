using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Experience", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x48)]
[PacketId(751, 754, 0x48)]
[PacketId(755, 759, 0x51)]
[PacketId(760, 760, 0x54)]
[PacketId(761, 761, 0x52)]
[PacketId(762, 763, 0x56)]
[PacketId(764, 764, 0x58)]
[PacketId(765, 765, 0x5A)]
[PacketId(766, 767, 0x5C)]
[PacketId(768, 769, 0x61)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x60)]
public sealed partial class ExperiencePacket : IServerPacket
{
    public float ExperienceBar { get; set; }
    public int Level { get; set; }
    public int TotalExperience { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteFloat(ExperienceBar);
                writer.WriteVarInt(Level);
                writer.WriteVarInt(TotalExperience);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ExperiencePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                ExperienceBar = reader.ReadFloat();
                Level = reader.ReadVarInt();
                TotalExperience = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ExperiencePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}