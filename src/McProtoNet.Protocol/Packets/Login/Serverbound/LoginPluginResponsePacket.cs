using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginPluginResponse", PacketState.Login, PacketDirection.Serverbound)]
public sealed partial class LoginPluginResponsePacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)
    };

    public int MessageId { get; set; }
    public byte[]? Data { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(MessageId);
                writer.WriteBoolean(Data is not null);
                if (Data is not null)
                {
                    writer.WriteBuffer(Data);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientLoginPacket.LoginPluginResponse), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                MessageId = reader.ReadVarInt();
                bool hasData = reader.ReadBoolean();
                Data = hasData ? reader.ReadRestBuffer() : null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientLoginPacket.LoginPluginResponse), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}