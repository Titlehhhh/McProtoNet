using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x1C, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerStatePacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPlayerStatePacket() { }


    }
}
