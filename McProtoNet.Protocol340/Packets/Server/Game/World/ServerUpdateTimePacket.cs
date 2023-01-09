namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerUpdateTimePacket : Packet<Protocol340>
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
