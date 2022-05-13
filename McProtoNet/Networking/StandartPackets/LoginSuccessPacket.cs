using McProtoNet.IO;
using McProtoNet.Networking;

namespace McProtoNet
{

    public sealed class LoginSuccessPacket : IPacket
    {
        public Guid UUID { get; set; }
        public string Username { get; set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {

            UUID = stream.ReadGuid();
            Username = stream.ReadString();
        }



        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public LoginSuccessPacket()
        {

        }
    }
}
