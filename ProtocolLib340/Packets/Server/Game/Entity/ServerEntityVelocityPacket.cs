using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityVelocityPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.motX = in.readShort() / 8000D;
        //this.motY = in.readShort() / 8000D;
        //this.motZ = in.readShort() / 8000D;
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityVelocityPacket() { }
    }

}
