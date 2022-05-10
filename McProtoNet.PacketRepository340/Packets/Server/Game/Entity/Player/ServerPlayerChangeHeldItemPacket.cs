using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerChangeHeldItemPacket : IPacket
    {
        //this.slot = in.readByte();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerChangeHeldItemPacket() { }
    }

}
