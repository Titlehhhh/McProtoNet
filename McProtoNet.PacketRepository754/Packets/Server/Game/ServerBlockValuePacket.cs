using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0A, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerBlockValuePacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerBlockValuePacket() { }
    }
}

