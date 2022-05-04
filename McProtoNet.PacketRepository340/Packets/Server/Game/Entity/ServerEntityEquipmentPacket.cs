using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityEquipmentPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.slot = MagicValues.key(EquipmentSlot.class, in.readVarInt());
        //this.item = NetUtil.readItem(in);
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityEquipmentPacket() { }
    }

}
