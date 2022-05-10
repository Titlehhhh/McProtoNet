using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.API
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
