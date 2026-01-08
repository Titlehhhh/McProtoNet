using System;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("StoreCookie", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class StoreCookiePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(766, MinecraftVersion.LatestProtocol)
    };

    public PacketCommonStoreCookie Data { get; set; } = null!;

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WritePacketCommonStoreCookie(Data, protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.StoreCookie), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonStoreCookie(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.StoreCookie), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}