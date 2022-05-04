using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientCreativeInventoryActionPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //out.writeShort(this.slot);
        //NetUtil.writeItem(out, this.clicked);
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
