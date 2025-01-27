using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.ClientboundPackets.Login;

public abstract class EncryptionBeginPacket : IServerPacket
{
    public string ServerId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }


    public sealed class V340_765 : EncryptionBeginPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ServerId = reader.ReadString();
            PublicKey = reader.ReadBuffer(reader.ReadVarInt());
            VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 340 and <= 765;
        }
    }

    public sealed class V766_769 : EncryptionBeginPacket
    {
        public bool ShouldAuthenticate { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ServerId = reader.ReadString();
            PublicKey = reader.ReadBuffer(reader.ReadVarInt());
            VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
            ShouldAuthenticate = reader.ReadBoolean();
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 766 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V340_765.SupportedVersion(protocolVersion) || V766_769.SupportedVersion(protocolVersion);
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    public static PacketIdentifier PacketId => ServerLoginPacket.EncryptionBegin;

    public PacketIdentifier GetPacketId() => PacketId;
}