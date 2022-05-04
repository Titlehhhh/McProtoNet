using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
