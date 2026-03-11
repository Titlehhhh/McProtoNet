using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("KeepAlive", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x03)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x04)]
public sealed partial class KeepAlivePacket : IClientPacket
{
    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("KeepAlivePacket 764-last fields missing.");
                writer.WriteVarInt(fields.Container);
                writer.WriteSignedLong(fields.KeepAliveId);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(KeepAlivePacket), protocolVersion, SupportedVersions);
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
                V764_Last = new V764_LastFields
                {
                    Container = reader.ReadVarInt(),
                    KeepAliveId = reader.ReadSignedLong()
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(KeepAlivePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public V764_LastFields? V764_Last { get; set; }

    public struct V764_LastFields
    {
        public int Container { get; set; }
        public long KeepAliveId { get; set; }
    }
}