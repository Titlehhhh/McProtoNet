using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Clientbound)]
public abstract partial class EncryptionBeginPacket : IServerPacket
{
    public string ServerId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }


    [PacketSubInfo(340, 765)]
    public sealed partial class V340_765 : EncryptionBeginPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ServerId = reader.ReadString();
            PublicKey = reader.ReadBuffer(reader.ReadVarInt());
            VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
        }
    }

    [PacketSubInfo(766, 769)]
    public sealed partial class V766_769 : EncryptionBeginPacket
    {
        public bool ShouldAuthenticate { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            ServerId = reader.ReadString();
            PublicKey = reader.ReadBuffer(reader.ReadVarInt());
            VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
            ShouldAuthenticate = reader.ReadBoolean();
        }
    }


    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
}