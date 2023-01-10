namespace McProtoNet.Protocol754.Packets.Client
{
    public sealed class StatusPingPacket : MinecraftPacket<Protocol754>
    {
        public long PayLoad { get; set; }

        public StatusPingPacket(long payLoad)
        {
            PayLoad = payLoad;
        }
        public StatusPingPacket()
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