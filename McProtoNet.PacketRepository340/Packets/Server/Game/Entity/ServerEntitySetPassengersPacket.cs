namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntitySetPassengersPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.passengerIds = new int[in.readVarInt()];
        //for(int index = 0; index < this.passengerIds.length; index++) {
        //this.passengerIds[index] = in.readVarInt();
        //}
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntitySetPassengersPacket() { }
    }

}
