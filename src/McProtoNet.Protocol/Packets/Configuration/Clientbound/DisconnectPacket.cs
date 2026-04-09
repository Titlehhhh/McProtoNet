using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Disconnect", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x01)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x02)]
public sealed partial class DisconnectPacket : IServerPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= 764:
            {
                var fields = V764_764 ?? throw new InvalidOperationException("DisconnectPacket 764-764 fields missing.");
                writer.WriteVarInt(fields.Container);
                writer.WriteString(fields.Reason);
                return;
            }
            case >= 765 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V765_Last ?? throw new InvalidOperationException("DisconnectPacket 765-last fields missing.");
                writer.WriteVarInt(fields.Container);
                writer.WriteAnonymousNbtTag(fields.Reason, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DisconnectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= 764:
                V764_764 = new V764_764Fields
                {
                    Container = reader.ReadVarInt(),
                    Reason = reader.ReadString()
                };
                return;
            case >= 765 and <= MinecraftVersion.LatestProtocol:
                V765_Last = new V765_LastFields
                {
                    Container = reader.ReadVarInt(),
                    Reason = reader.ReadAnonymousNbtTag(protocolVersion)
                };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(DisconnectPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public V764_764Fields? V764_764 { get; set; }
    public V765_LastFields? V765_Last { get; set; }

    public struct V764_764Fields
    {
        public int Container { get; set; }
        public string Reason { get; set; }
    }

    public struct V765_LastFields
    {
        public int Container { get; set; }
        public NbtTag Reason { get; set; }
    }
}