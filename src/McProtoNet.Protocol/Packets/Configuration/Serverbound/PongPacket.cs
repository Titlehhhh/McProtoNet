using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("Pong", PacketState.Configuration, PacketDirection.Serverbound)]
public sealed partial class PongPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, MinecraftVersion.LatestProtocol)
    };

    public int Id { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                writer.WriteSignedInt(Id);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.Pong), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                Id = reader.ReadSignedInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.Pong), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}