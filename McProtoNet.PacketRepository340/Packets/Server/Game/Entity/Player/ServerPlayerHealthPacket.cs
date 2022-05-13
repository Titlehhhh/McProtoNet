namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerHealthPacket : IPacket
    {
        //this.health = in.readFloat();
        //this.food = in.readVarInt();
        //this.saturation = in.readFloat();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerHealthPacket() { }
    }

}
