using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x01)]
public sealed partial class EncryptionBeginPacket : IServerPacket
{
    public string ServerId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                writer.WriteString(ServerId);
                writer.WriteBuffer(PublicKey, LengthFormat.VarInt);
                writer.WriteBuffer(VerifyToken, LengthFormat.VarInt);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("EncryptionBeginPacket 766-last fields missing.");
                writer.WriteString(ServerId);
                writer.WriteBuffer(PublicKey, LengthFormat.VarInt);
                writer.WriteBuffer(VerifyToken, LengthFormat.VarInt);
                writer.WriteBoolean(fields.ShouldAuthenticate);
                return;
            }
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
            {
                ServerId = reader.ReadString();
                PublicKey = reader.ReadBuffer(LengthFormat.VarInt);
                VerifyToken = reader.ReadBuffer(LengthFormat.VarInt);
                V766_Last = null;
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                ServerId = reader.ReadString();
                PublicKey = reader.ReadBuffer(LengthFormat.VarInt);
                VerifyToken = reader.ReadBuffer(LengthFormat.VarInt);
                V766_Last = new V766_LastFields { ShouldAuthenticate = reader.ReadBoolean() };
                return;
            }
        }
    }

    public struct V766_LastFields
    {
        public bool ShouldAuthenticate { get; set; }
    }
}