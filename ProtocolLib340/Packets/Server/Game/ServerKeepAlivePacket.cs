using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerKeepAlivePacket : IPacket
    {
        public long ID { get; set; }

        public void Read(IMinecraftStreamReader stream)
        {
            ID = stream.ReadLong();
        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerKeepAlivePacket(long iD)
        {
            ID = iD;
        }

        public ServerKeepAlivePacket() { }
    }

}
