namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerUpdateTimePacket : Packet
    {
        //this.age = in.readLong();
        //this.time = in.readLong();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUpdateTimePacket() { }
    }

}
