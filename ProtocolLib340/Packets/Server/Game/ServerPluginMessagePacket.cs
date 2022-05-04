using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerPluginMessagePacket : IPacket
    {
        //this.channel = in.readString();
        //this.data = in.readBytes(in.available());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPluginMessagePacket() { }
    }

}
