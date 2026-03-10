using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("BlockChange", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0B)]
[PacketId(751, 754, 0x0B)]
[PacketId(755, 758, 0x0C)]
[PacketId(759, 761, 0x09)]
[PacketId(762, 763, 0x0A)]
[PacketId(764, 769, 0x09)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x08)]
public sealed partial class BlockChangePacket : IServerPacket
{
    public Position Location { get; set; }
    public int Type { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteType(Location, protocolVersion);
                writer.WriteVarInt(Type);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockChangePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Location = reader.ReadType<Position>(protocolVersion);
                Type = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(BlockChangePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}