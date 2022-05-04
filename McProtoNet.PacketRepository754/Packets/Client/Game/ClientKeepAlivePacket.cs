using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x10, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientKeepAlivePacket : IPacket
    {
        public long PingId { get; private set; }

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
