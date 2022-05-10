using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerWindowPropertyPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        //this.property = in.readShort();
        //this.value = in.readShort();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerWindowPropertyPacket() { }
    }

}
