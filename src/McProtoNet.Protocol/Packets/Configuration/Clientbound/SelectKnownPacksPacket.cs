using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("SelectKnownPacks", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class SelectKnownPacksPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(766, MinecraftVersion.LatestProtocol)
    };

    public PacketCommonSelectKnownPacks? Data { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var data = Data ?? throw new InvalidOperationException("SelectKnownPacks data missing.");
                writer.WritePacketCommonSelectKnownPacks(data, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.SelectKnownPacks), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonSelectKnownPacks(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.SelectKnownPacks), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
