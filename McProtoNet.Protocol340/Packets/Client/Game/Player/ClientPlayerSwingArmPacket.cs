namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientPlayerSwingArmPacket : MinecraftPacket<Protocol340>
    {
         

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeVarInt(MagicValues.value(Integer.class, this.hand));
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
