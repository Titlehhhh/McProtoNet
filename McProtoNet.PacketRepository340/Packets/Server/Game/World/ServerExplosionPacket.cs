namespace McProtoNet.PacketRepository340.Packets.Server
{


    public sealed class ServerExplosionPacket : Packet
    {
        //this.x = in.readFloat();
        //this.y = in.readFloat();
        //this.z = in.readFloat();
        //this.radius = in.readFloat();
        //this.exploded = new ArrayList<ExplodedBlockRecord>();
        //int length = in.readInt();
        //for(int count = 0; count < length; count++) {
        //this.exploded.add(new ExplodedBlockRecord(in.readByte(), in.readByte(), in.readByte()));
        //}
        //
        //this.pushX = in.readFloat();
        //this.pushY = in.readFloat();
        //this.pushZ = in.readFloat();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerExplosionPacket() { }
    }

}
