using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Abilities", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x31)]
[PacketId(751, 754, 0x30)]
[PacketId(755, 758, 0x32)]
[PacketId(759, 759, 0x2F)]
[PacketId(760, 760, 0x31)]
[PacketId(761, 761, 0x30)]
[PacketId(762, 763, 0x34)]
[PacketId(764, 765, 0x36)]
[PacketId(766, 767, 0x38)]
[PacketId(768, 769, 0x3A)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x39)]
public sealed partial class AbilitiesPacket : IServerPacket
{
    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteSignedByte(Flags);
                writer.WriteFloat(FlyingSpeed);
                writer.WriteFloat(WalkingSpeed);
                return;
            }
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
            {
                Flags = reader.ReadSignedByte();
                FlyingSpeed = reader.ReadFloat();
                WalkingSpeed = reader.ReadFloat();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AbilitiesPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public sbyte Flags { get; set; }
    public float FlyingSpeed { get; set; }
    public float WalkingSpeed { get; set; }
}