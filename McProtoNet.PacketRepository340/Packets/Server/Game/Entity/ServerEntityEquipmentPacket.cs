namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityEquipmentPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.slot = MagicValues.key(EquipmentSlot.class, in.readVarInt());
        //this.item = NetUtil.readItem(in);
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityEquipmentPacket() { }
    }

}
