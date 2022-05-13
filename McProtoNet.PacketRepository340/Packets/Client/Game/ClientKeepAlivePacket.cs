namespace McProtoNet.PacketRepository340.Packets.Client
{


    public class ClientKeepAlivePacket : IPacket
    {
        public long ID { get; set; }
        //out.writeLong(this.id);
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteLong(ID);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientKeepAlivePacket(long iD)
        {
            ID = iD;
        }
    }
}
