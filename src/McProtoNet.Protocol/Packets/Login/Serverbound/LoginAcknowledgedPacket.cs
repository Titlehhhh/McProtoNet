using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginAcknowledged", PacketState.Login, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class LoginAcknowledgedPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                return;
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("LoginAcknowledgedPacket 764-765 fields missing.");
                writer.WriteVarInt(fields.Container);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("LoginAcknowledgedPacket 766-last fields missing.");
                writer.WriteUnsignedByte(fields.Container);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(LoginAcknowledgedPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 763:
                V764_765 = null;
                V766_Last = null;
                return;
            case >= 764 and <= 765:
                V764_765 = new V764_765Fields { Container = reader.ReadVarInt() };
                V766_Last = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                V764_765 = null;
                V766_Last = new V766_LastFields { Container = reader.ReadUnsignedByte() };
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(LoginAcknowledgedPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public V764_765Fields? V764_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    public struct V764_765Fields { public int Container { get; set; } }
    public struct V766_LastFields { public byte Container { get; set; } }
}