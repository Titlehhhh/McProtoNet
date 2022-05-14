namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerEntityDestroyPacket : Packet
    {
        //this.entityIds = new int[in.readVarInt()];
        //for(int index = 0; index < this.entityIds.length; index++) {
        //this.entityIds[index] = in.readVarInt();
        //}
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityDestroyPacket() { }
    }

}
