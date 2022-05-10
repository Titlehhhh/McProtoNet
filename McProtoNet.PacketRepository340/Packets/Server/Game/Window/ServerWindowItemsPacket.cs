using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerWindowItemsPacket : IPacket
    {
        //this.windowId = in.readUnsignedByte();
        //this.items = new ItemStack[in.readShort()];
        //for(int index = 0; index < this.items.length; index++) {
        //this.items[index] = NetUtil.readItem(in);
        //}
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerWindowItemsPacket() { }
    }

}
