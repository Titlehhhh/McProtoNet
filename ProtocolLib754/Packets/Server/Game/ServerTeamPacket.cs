using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Server
{

    [PacketInfo(0x4C, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerTeamPacket : IPacket
    {
        public void Write(IMinecraftStreamWriter stream)
        {

        }
        public void Read(IMinecraftStreamReader stream)
        {

        }
        public ServerTeamPacket() { }
    }
}

