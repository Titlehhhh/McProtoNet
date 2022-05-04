using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerUnloadChunkPacket : IPacket
    {
        //this.x = in.readInt();
        //this.z = in.readInt();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerUnloadChunkPacket() { }
    }

}
