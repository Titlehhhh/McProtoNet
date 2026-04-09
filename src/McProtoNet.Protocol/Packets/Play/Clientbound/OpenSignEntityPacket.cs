using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("OpenSignEntity", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 762)]
[ProtocolSupport(763, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2F)]
[PacketId(751, 754, 0x2E)]
[PacketId(755, 758, 0x2F)]
[PacketId(759, 759, 0x2C)]
[PacketId(760, 760, 0x2E)]
[PacketId(761, 761, 0x2D)]
[PacketId(762, 763, 0x31)]
[PacketId(764, 765, 0x32)]
[PacketId(766, 767, 0x34)]
[PacketId(768, 769, 0x36)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x35)]
public sealed partial class OpenSignEntityPacket : IServerPacket
{
    public Position Location { get; set; }
    public V763_LastFields? V763_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
                writer.WriteType(Location, protocolVersion);
                return;
            case >= 763 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V763_Last ?? throw new InvalidOperationException("OpenSignEntityPacket 763-last fields missing.");
                writer.WriteType(Location, protocolVersion);
                writer.WriteBoolean(fields.IsFrontText);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenSignEntityPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 762:
                Location = reader.ReadType<Position>(protocolVersion);
                V763_Last = null;
                return;
            case >= 763 and <= MinecraftVersion.LatestProtocol:
                Location = reader.ReadType<Position>(protocolVersion);
                V763_Last = new V763_LastFields { IsFrontText = reader.ReadBoolean() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(OpenSignEntityPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V763_LastFields
    {
        public bool IsFrontText { get; set; }
    }
}