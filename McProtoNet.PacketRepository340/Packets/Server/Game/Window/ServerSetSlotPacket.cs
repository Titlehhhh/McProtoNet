using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerSetSlotPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        //this.slot = in.readShort();
        //this.item = NetUtil.readItem(in);
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSetSlotPacket() { }
    }

}
