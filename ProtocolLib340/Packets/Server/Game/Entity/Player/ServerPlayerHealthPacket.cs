using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerPlayerHealthPacket : IPacket
    {
        //this.health = in.readFloat();
        //this.food = in.readVarInt();
        //this.saturation = in.readFloat();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPlayerHealthPacket() { }
    }

}
