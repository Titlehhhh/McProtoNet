using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityRemoveEffectPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.effect = MagicValues.key(Effect.class, in.readUnsignedByte());
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityRemoveEffectPacket() { }
    }

}
