using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerSpawnExpOrbPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.exp = in.readShort();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnExpOrbPacket() { }
    }

}
