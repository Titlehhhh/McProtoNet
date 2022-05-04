using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
