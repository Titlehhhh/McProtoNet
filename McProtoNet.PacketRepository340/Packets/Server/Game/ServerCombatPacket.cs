namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerCombatPacket : IPacket
    {
        //this.state = MagicValues.key(CombatState.class, in.readVarInt());
        //if(this.state == CombatState.END_COMBAT) {
        //this.duration = in.readVarInt();
        //this.entityId = in.readInt();
        //} else if(this.state == CombatState.ENTITY_DEAD) {
        //this.playerId = in.readVarInt();
        //this.entityId = in.readInt();
        //this.message = Message.fromString(in.readString());
        //}
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerCombatPacket() { }
    }

}
