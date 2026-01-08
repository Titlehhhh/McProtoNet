using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("CustomPayload", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class CustomPayloadPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, MinecraftVersion.LatestProtocol)
    };
    public string Channel { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                writer.WriteString(Channel);
                writer.WriteBuffer(Data);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.CustomPayload), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                Channel = reader.ReadString();
                Data = reader.ReadRestBuffer();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.CustomPayload), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}