using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Server
{

    public sealed class LoginDisconnectPacket : IMinecraftPacket
    {
        public string Message { get; set; }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Message);
        }

        public LoginDisconnectPacket()
        {

        }

        public LoginDisconnectPacket(string message)
        {
            Message = message;
        }
    }
}
