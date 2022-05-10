using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityAnimationPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.animation = MagicValues.key(Animation.class, in.readUnsignedByte());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityAnimationPacket() { }
    }

}
