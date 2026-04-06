using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("SpawnPosition", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x42)]
[PacketId(751, 754, 0x42)]
[PacketId(755, 758, 0x4B)]
[PacketId(759, 759, 0x4A)]
[PacketId(760, 760, 0x4D)]
[PacketId(761, 761, 0x4C)]
[PacketId(762, 763, 0x50)]
[PacketId(764, 764, 0x52)]
[PacketId(765, 765, 0x54)]
[PacketId(766, 767, 0x56)]
[PacketId(768, 769, 0x5B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x5A)]
public sealed partial class SpawnPositionPacket : IServerPacket
{
    public Position Location { get; set; }
    public V755_LastFields? V755_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                writer.WriteType(Location, protocolVersion);
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V755_Last ?? throw new InvalidOperationException("SpawnPositionPacket 755-last fields missing.");
                writer.WriteType(Location, protocolVersion);
                writer.WriteFloat(fields.Angle);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SpawnPositionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                Location = reader.ReadType<Position>(protocolVersion);
                V755_Last = null;
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
                Location = reader.ReadType<Position>(protocolVersion);
                V755_Last = new V755_LastFields { Angle = reader.ReadFloat() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SpawnPositionPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V755_LastFields { public float Angle { get; set; } }
}