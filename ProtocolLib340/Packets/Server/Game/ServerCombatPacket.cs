using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
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
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerCombatPacket() { }
    }

}
