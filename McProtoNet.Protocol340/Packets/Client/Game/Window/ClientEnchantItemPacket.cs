namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientEnchantItemPacket : MinecraftPacket<Protocol340>
    {
         

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeByte(this.windowId);
        //out.writeByte(this.enchantment);
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
