using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityAttachPacket : IPacket
    {
        //this.entityId = in.readInt();
        //this.attachedToId = in.readInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityAttachPacket() { }
    }

}
