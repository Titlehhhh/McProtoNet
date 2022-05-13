using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class LoginSetCompressionPacket : IPacket
    {
        public int Threshold { get; set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            Threshold = stream.ReadVarInt();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public LoginSetCompressionPacket()
        {

        }
    }
}
