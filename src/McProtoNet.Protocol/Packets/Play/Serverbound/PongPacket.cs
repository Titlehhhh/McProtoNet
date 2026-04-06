using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("Pong", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x1D)]
[PacketId(759, 759, 0x1F)]
[PacketId(760, 760, 0x20)]
[PacketId(761, 761, 0x1F)]
[PacketId(762, 763, 0x20)]
[PacketId(764, 764, 0x23)]
[PacketId(765, 765, 0x24)]
[PacketId(766, 767, 0x27)]
[PacketId(768, 768, 0x29)]
[PacketId(769, 770, 0x2B)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2C)]
public sealed partial class PongPacket : IClientPacket
{
    public V755_LastFields? V755_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V755_Last ?? throw new InvalidOperationException("PongPacket 755-last fields missing.");
                writer.WriteSignedInt(fields.Id);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PongPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
                V755_Last = new V755_LastFields { Id = reader.ReadSignedInt() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PongPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V755_LastFields
    {
        public int Id { get; set; }
    }
}