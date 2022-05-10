using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x0E, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerInteractEntityPacket : IPacket
    {


        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPlayerInteractEntityPacket() { }


    }
}
