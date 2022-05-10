using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityMetadataPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.metadata = NetUtil.readEntityMetadata(in);
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityMetadataPacket() { }
    }

}
