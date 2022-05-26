namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerBlockChangePacket : Packet
    {
        //this.record = new BlockChangeRecord(NetUtil.readPosition(in), NetUtil.readBlockState(in));
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerBlockChangePacket() { }
    }

}
