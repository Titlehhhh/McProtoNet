using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
