using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x19, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerDisconnectPacket : IPacket
    {
        public string Message { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Message);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }
        public ServerDisconnectPacket() { }
    }
}

