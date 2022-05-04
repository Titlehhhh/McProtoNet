using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerAdvancementTabPacket : IPacket
    {
        //if(in.readBoolean()) {
        //this.tabId = in.readString();
        //} else {
        //this.tabId = null;
        //}
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerAdvancementTabPacket() { }
    }

}
