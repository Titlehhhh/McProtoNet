using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerUnloadChunkPacket : IPacket
    {
        //this.x = in.readInt();
        //this.z = in.readInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUnloadChunkPacket() { }
    }

}
