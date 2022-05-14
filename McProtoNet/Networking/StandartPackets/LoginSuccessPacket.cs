using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class LoginSuccessPacket : Packet
    {
        public Guid UUID { get; set; }
        public string Username { get; set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {

            UUID = stream.ReadGuid();
            Username = stream.ReadString();
        }



        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public LoginSuccessPacket()
        {

        }
    }
}
