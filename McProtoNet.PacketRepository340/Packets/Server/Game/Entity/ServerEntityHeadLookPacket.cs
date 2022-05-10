using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityHeadLookPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.headYaw = in.readByte() * 360 / 256f;
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityHeadLookPacket() { }
    }

}
