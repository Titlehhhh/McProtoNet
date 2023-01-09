namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientCreativeInventoryActionPacket : Packet<Protocol340>
    {
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeShort(this.slot);
        //NetUtil.writeItem(out, this.clicked);
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
