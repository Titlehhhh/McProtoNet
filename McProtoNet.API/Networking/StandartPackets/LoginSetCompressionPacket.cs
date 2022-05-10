using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.API
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
