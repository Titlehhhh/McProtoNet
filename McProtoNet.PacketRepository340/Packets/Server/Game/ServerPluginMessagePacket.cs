using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPluginMessagePacket : IPacket
    {
        //this.channel = in.readString();
        //this.data = in.readBytes(in.available());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPluginMessagePacket() { }
    }

}
