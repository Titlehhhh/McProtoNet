using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientEnchantItemPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //out.writeByte(this.windowId);
        //out.writeByte(this.enchantment);
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
