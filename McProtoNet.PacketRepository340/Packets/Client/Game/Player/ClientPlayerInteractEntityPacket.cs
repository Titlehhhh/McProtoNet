using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerInteractEntityPacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeVarInt(this.entityId);
        //out.writeVarInt(MagicValues.value(Integer.class, this.action));
        //if(this.action == InteractAction.INTERACT_AT) {
        //out.writeFloat(this.targetX);
        //out.writeFloat(this.targetY);
        //out.writeFloat(this.targetZ);
        //}
        //
        //if(this.action == InteractAction.INTERACT || this.action == InteractAction.INTERACT_AT) {
        //out.writeVarInt(MagicValues.value(Integer.class, this.hand));
        //}
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
