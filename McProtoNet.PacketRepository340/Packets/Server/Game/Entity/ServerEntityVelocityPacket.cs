using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityVelocityPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.motX = in.readShort() / 8000D;
        //this.motY = in.readShort() / 8000D;
        //this.motZ = in.readShort() / 8000D;
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityVelocityPacket() { }
    }

}
