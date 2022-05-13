namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerKeepAlivePacket : IPacket
    {
        public long ID { get; set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            ID = stream.ReadLong();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerKeepAlivePacket(long iD)
        {
            ID = iD;
        }

        public ServerKeepAlivePacket() { }
    }

}
