using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Client
{

    public sealed class EncryptionResponsePacket : IMinecraftPacket
    {
        public byte[] SharedKey { get; private set; }
        public byte[] VerifyToken { get; private set; }

        public EncryptionResponsePacket(byte[] sharedKey, byte[] verifyToken)
        {
            SharedKey = sharedKey;
            VerifyToken = verifyToken;
        }

        public  void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteByteArray(SharedKey);
            stream.WriteByteArray(VerifyToken);
        }

        public  void Read(IMinecraftPrimitiveReader stream)
        {
            SharedKey = stream.ReadByteArray();
            VerifyToken = stream.ReadByteArray();
        }
        public EncryptionResponsePacket()
        {

        }

    }
}
