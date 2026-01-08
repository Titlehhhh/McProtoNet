using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginAcknowledged", PacketState.Login, PacketDirection.Serverbound)]
public sealed partial class LoginAcknowledgedPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, MinecraftVersion.LatestProtocol)
    };

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientLoginPacket.LoginAcknowledged), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientLoginPacket.LoginAcknowledged), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}