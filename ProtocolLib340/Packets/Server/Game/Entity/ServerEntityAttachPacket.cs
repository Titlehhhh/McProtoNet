using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityAttachPacket : IPacket
    {
        //this.entityId = in.readInt();
        //this.attachedToId = in.readInt();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityAttachPacket() { }
    }

}
