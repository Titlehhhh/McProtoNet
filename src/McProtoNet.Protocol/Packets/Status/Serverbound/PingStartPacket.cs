using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Status.Serverbound;

[PacketInfo("PingStart", PacketState.Status, PacketDirection.Serverbound)]
public sealed partial class PingStartPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)
    };

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientStatusPacket.PingStart), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientStatusPacket.PingStart), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}