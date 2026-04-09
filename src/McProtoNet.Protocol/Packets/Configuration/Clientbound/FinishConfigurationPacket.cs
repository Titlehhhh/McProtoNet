using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("FinishConfiguration", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x02)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class FinishConfigurationPacket : IServerPacket
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
                var fields = V764_Last ?? throw new InvalidOperationException("FinishConfigurationPacket 764-last fields missing.");
                writer.WriteVarInt(fields.Container);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(FinishConfigurationPacket), protocolVersion, SupportedVersions);
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
                V764_Last = new V764_LastFields { Container = reader.ReadVarInt() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(FinishConfigurationPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V764_LastFields
    {
        public int Container { get; set; }
    }
}