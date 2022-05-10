using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerResourcePackSendPacket : IPacket
    {
        //this.url = in.readString();
        //this.hash = in.readString();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerResourcePackSendPacket() { }
    }

}
