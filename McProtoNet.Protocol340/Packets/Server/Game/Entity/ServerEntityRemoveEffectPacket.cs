namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityRemoveEffectPacket : Packet<Protocol340>
    {
        //this.entityId = in.readVarInt();
        //this.effect = MagicValues.key(Effect.class, in.readUnsignedByte());
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityRemoveEffectPacket() { }
    }

}
