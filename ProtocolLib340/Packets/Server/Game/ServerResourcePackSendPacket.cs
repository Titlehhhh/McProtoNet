using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerResourcePackSendPacket : IPacket
    {
        //this.url = in.readString();
        //this.hash = in.readString();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerResourcePackSendPacket() { }
    }

}
