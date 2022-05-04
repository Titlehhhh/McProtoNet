using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
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
