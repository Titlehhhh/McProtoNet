using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class LoginSetCompressionPacket : Packet
    {
        public int Threshold { get; set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Threshold = stream.ReadVarInt();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public LoginSetCompressionPacket()
        {

        }
    }
}
