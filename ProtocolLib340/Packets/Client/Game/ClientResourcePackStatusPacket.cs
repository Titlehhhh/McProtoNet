using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientResourcePackStatusPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //out.writeVarInt(MagicValues.value(Integer.class, this.status));
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
