using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x59, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerEntityEffectPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ServerEntityEffectPacket() { }
    }
}

