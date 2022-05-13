

namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x03, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientChatPacket : IPacket
    {
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientChatPacket() { }
    }
}

