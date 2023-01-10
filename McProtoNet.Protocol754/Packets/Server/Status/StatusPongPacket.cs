namespace McProtoNet.Protocol754.Packets.Server
{
    public sealed class StatusPongPacket : MinecraftPacket<Protocol754>
    {
        public long PayLoad { get; set; }

        public StatusPongPacket(long payLoad)
        {
            PayLoad = payLoad;
        }
        public StatusPongPacket()
        {

        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            PayLoad = stream.ReadLong();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteLong(PayLoad);
        }
    }
}
