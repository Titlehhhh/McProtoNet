using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Clientbound)]
public sealed partial class EncryptionBeginPacket : IServerPacket
{
    public string ServerId { get; set; } = string.Empty;
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();
    public byte[] VerifyToken { get; set; } = Array.Empty<byte>();

    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                writer.WriteString(ServerId);
                writer.WriteVarInt(PublicKey.Length);
                writer.WriteBuffer(PublicKey);
                writer.WriteVarInt(VerifyToken.Length);
                writer.WriteBuffer(VerifyToken);
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("EncryptionBegin V766_Last missing.");
                writer.WriteString(ServerId);
                writer.WriteVarInt(PublicKey.Length);
                writer.WriteBuffer(PublicKey);
                writer.WriteVarInt(VerifyToken.Length);
                writer.WriteBuffer(VerifyToken);
                writer.WriteBoolean(fields.ShouldAuthenticate);
                return;
            }
            default:
                throw new ProtocolNotSupportException(nameof(ServerLoginPacket.EncryptionBegin), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                ServerId = reader.ReadString();
                PublicKey = reader.ReadBuffer(reader.ReadVarInt());
                VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                ServerId = reader.ReadString();
                PublicKey = reader.ReadBuffer(reader.ReadVarInt());
                VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
                V766_Last = new V766_LastFields
                {
                    ShouldAuthenticate = reader.ReadBoolean()
                };
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ServerLoginPacket.EncryptionBegin), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V766_LastFields
    {
        public bool ShouldAuthenticate { get; set; }
    }
}
