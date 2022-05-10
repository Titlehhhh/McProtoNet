using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityStatusPacket : IPacket
    {
        //this.entityId = in.readInt();
        //this.status = MagicValues.key(EntityStatus.class, in.readByte());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityStatusPacket() { }
    }

}
