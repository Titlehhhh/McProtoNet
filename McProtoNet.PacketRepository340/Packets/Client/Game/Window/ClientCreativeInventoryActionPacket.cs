using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientCreativeInventoryActionPacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeShort(this.slot);
        //NetUtil.writeItem(out, this.clicked);
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
