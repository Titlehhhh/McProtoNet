using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerUpdateTimePacket : IPacket
    {
        //this.age = in.readLong();
        //this.time = in.readLong();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerUpdateTimePacket() { }
    }

}
