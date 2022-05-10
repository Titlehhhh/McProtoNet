using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerUseBedPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.position = NetUtil.readPosition(in);
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerUseBedPacket() { }
    }

}
