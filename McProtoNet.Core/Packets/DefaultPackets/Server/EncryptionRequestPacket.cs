using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Server
{
    public sealed class EncryptionRequestPacket : Packet
    {
        public string ServerId { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            ServerId = stream.ReadString();
            PublicKey = stream.ReadByteArray();
            VerifyToken = stream.ReadByteArray();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(ServerId);
            stream.WriteByteArray(PublicKey);
            stream.WriteByteArray(VerifyToken);
        }
    }
}
