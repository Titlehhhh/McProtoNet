using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{
    public sealed class EncryptionRequestPacket : IPacket
    {
        public string ServerId { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            ServerId = stream.ReadString();
            PublicKey = stream.ReadUInt8Array(stream.ReadVarInt());
            VerifyToken = stream.ReadUInt8Array(stream.ReadVarInt());
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(ServerId);
            stream.WriteVarInt(PublicKey.Length);
            stream.Write(PublicKey);
            stream.WriteVarInt(VerifyToken.Length);
            stream.Write(VerifyToken);
        }
    }
}
