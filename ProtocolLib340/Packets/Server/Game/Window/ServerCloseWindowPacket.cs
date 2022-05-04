using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerCloseWindowPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerCloseWindowPacket() { }
    }

}
