using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerDisconnectPacket : IPacket
    {
        //this.message = Message.fromString(in.readString());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerDisconnectPacket() { }
    }

}
