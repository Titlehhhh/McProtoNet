

using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
{

    public sealed class EncryptionResponsePacket : Packet
    {
        public byte[] SharedKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public EncryptionResponsePacket(byte[] sharedKey, byte[] verifyToken)
        {
            SharedKey = sharedKey;
            VerifyToken = verifyToken;
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteByteArray(SharedKey);
            stream.WriteByteArray(VerifyToken);
        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            SharedKey = stream.ReadByteArray();
            VerifyToken = stream.ReadByteArray();
        }
        public EncryptionResponsePacket()
        {

        }

    }
}
