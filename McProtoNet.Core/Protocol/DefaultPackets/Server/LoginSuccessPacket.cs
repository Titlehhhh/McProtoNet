using McProtoNet.Core.IO;

namespace McProtoNet.Core.Protocol
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
            stream.WriteUuid(UUID);
            stream.WriteString(Username);
        }

        public LoginSuccessPacket(Guid uUID, string username)
        {
            UUID = uUID;
            Username = username;
        }

        public LoginSuccessPacket()
        {

        }
    }
}
