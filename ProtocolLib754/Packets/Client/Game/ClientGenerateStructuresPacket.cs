using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x0F, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientGenerateStructuresPacket : IPacket
    {


        public void Write(IMinecraftStreamWriter stream)
        {

        }
        public void Read(IMinecraftStreamReader stream)
        {

        }
        public ClientGenerateStructuresPacket() { }


    }
}
