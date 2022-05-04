using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x1F, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerKeepAlivePacket : IPacket
    {
        public long PingID { get; set; }
        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteLong(PingID);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            PingID = stream.ReadLong();
        }
        public ServerKeepAlivePacket() { }
    }
}

