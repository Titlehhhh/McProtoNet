using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
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
