namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityAnimationPacket : Packet<Protocol340>
    {
        //this.entityId = in.readVarInt();
        //this.animation = MagicValues.key(Animation.class, in.readUnsignedByte());
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityAnimationPacket() { }
    }

}
