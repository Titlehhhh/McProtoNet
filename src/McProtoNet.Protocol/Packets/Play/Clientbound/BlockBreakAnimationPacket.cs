using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("BlockBreakAnimation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x08)]
[PacketId(751, 754, 0x08)]
[PacketId(755, 758, 0x09)]
[PacketId(759, 761, 0x06)]
[PacketId(762, 763, 0x07)]
[PacketId(764, 769, 0x06)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x05)]
public sealed partial class BlockBreakAnimationPacket : IServerPacket
{
    public int EntityId { get; set; }
    public Position Location { get; set; }
    public sbyte DestroyStage { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(EntityId);
                writer.WriteType(Location, protocolVersion);
                writer.WriteSignedByte(DestroyStage);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockBreakAnimationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                EntityId = reader.ReadVarInt();
                Location = reader.ReadType<Position>(protocolVersion);
                DestroyStage = reader.ReadSignedByte();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockBreakAnimationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}