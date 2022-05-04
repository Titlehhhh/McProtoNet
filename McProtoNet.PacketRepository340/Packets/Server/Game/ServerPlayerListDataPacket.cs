using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerListDataPacket : IPacket
    {
        //this.header = Message.fromString(in.readString());
        //this.footer = Message.fromString(in.readString());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPlayerListDataPacket() { }
    }

}
