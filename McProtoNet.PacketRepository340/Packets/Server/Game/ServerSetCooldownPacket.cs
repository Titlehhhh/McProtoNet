using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerSetCooldownPacket : IPacket
    {
        //this.itemId = in.readVarInt();
        //this.cooldownTicks = in.readVarInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSetCooldownPacket() { }
    }

}
