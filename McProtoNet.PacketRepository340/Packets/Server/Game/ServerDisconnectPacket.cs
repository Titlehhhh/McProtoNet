using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerDisconnectPacket : IPacket
    {

        public string Messaage { get; private set; }
        //this.message = Message.fromString(in.readString());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerDisconnectPacket() { }
    }

}
