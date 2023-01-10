namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityStatusPacket : MinecraftPacket<Protocol340>
    {
         

        //this.entityId = in.readInt();
        //this.status = MagicValues.key(EntityStatus.class, in.readByte());
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityStatusPacket() { }
    }

}
