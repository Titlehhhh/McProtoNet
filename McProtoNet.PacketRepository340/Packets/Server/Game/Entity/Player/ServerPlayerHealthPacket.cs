namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerPlayerHealthPacket : Packet
    {
        //this.health = in.readFloat();
        //this.food = in.readVarInt();
        //this.saturation = in.readFloat();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerHealthPacket() { }
    }

}
