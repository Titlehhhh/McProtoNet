using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerSpawnGlobalEntityPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.type = MagicValues.key(GlobalEntityType.class, in.readByte());
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnGlobalEntityPacket() { }
    }

}
