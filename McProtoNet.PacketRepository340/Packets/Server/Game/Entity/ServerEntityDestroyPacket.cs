namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityDestroyPacket : IPacket
    {
        //this.entityIds = new int[in.readVarInt()];
        //for(int index = 0; index < this.entityIds.length; index++) {
        //this.entityIds[index] = in.readVarInt();
        //}
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityDestroyPacket() { }
    }

}
