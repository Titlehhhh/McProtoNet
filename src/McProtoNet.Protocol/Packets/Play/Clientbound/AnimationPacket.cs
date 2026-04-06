using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Animation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x05)]
[PacketId(751, 754, 0x05)]
[PacketId(755, 758, 0x06)]
[PacketId(759, 761, 0x03)]
[PacketId(762, 763, 0x04)]
[PacketId(764, 769, 0x03)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class AnimationPacket : IServerPacket
{
    public int EntityId { get; set; }
    public byte Animation { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                writer.WriteUnsignedByte(Animation);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AnimationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                EntityId = reader.ReadVarInt();
                Animation = reader.ReadUnsignedByte();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(AnimationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}