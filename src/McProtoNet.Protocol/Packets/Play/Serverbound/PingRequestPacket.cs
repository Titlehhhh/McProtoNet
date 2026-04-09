using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("PingRequest", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 764, 0x1D)]
[PacketId(765, 765, 0x1E)]
[PacketId(766, 767, 0x21)]
[PacketId(768, 768, 0x23)]
[PacketId(769, 770, 0x24)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x25)]
public sealed partial class PingRequestPacket : IClientPacket
{
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("PingRequestPacket 764-last fields missing.");
                writer.WriteSignedLong(fields.Id);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PingRequestPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                V764_Last = new V764_LastFields { Id = reader.ReadSignedLong() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(PingRequestPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V764_LastFields
    {
        public long Id { get; set; }
    }
}