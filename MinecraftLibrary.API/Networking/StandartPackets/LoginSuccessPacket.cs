using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;

namespace ProtoLib.API
{
    
    public sealed class LoginSuccessPacket : IPacket
    {
        public Guid UUID { get; set; }
        public string Username { get; set; }

        public void Read(IMinecraftStreamReader stream)
        {

            UUID = stream.ReadGuid();
            Username = stream.ReadString();
        }



        public void Write(IMinecraftStreamWriter stream)
        {

        }
        public LoginSuccessPacket()
        {

        }
    }
}
