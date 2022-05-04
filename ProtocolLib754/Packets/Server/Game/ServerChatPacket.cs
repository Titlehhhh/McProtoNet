using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Server
{

    [PacketInfo(0x0E, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerChatPacket : IPacket
    {
        public string Message { get; set; }
        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteString(Message);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            Message = stream.ReadString();
        }
        public ServerChatPacket() { }
    }
}

