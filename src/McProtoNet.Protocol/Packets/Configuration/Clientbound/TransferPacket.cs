using System;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Transfer", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class TransferPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(771, MinecraftVersion.LatestProtocol)
    };

    public PacketCommonTransfer Data { get; set; } = null!;

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WritePacketCommonTransfer(Data, protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.Transfer), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonTransfer(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.Transfer), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}