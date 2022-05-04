using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{
    
    [PacketInfo(0x10, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientKeepAlivePacket : IPacket
    {
        public long PingId {get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteLong(PingId);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            PingId = stream.ReadLong();
        }
        public ClientKeepAlivePacket() { }

        public ClientKeepAlivePacket(long PingId)
        {
            this.PingId = PingId;
        }
    }
}
