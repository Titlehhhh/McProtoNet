using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class EncryptionResponsePacket : IPacket
    {
        public byte[] SharedKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public EncryptionResponsePacket(byte[] sharedKey, byte[] verifyToken)
        {
            SharedKey = sharedKey;
            VerifyToken = verifyToken;
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteByteArray(SharedKey);
            stream.WriteByteArray(VerifyToken);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            SharedKey = stream.ReadByteArray();
            VerifyToken = stream.ReadByteArray();
        }
        public EncryptionResponsePacket()
        {

        }

    }
}
