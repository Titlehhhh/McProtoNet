using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("LoginPluginRequest", PacketState.Login, PacketDirection.Clientbound)]
public sealed partial class LoginPluginRequestPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)
    };

    public int MessageId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(MessageId);
                writer.WriteString(Channel);
                writer.WriteBuffer(Data);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerLoginPacket.LoginPluginRequest), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                MessageId = reader.ReadVarInt();
                Channel = reader.ReadString();
                Data = reader.ReadRestBuffer();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerLoginPacket.LoginPluginRequest), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}