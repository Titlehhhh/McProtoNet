namespace McProtoNet.Protocol754.Packets.Server
{

    
    public sealed class ServerKeepAlivePacket : Packet 
    {
        public long PingID { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteLong(PingID);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            PingID = stream.ReadLong();
        }
        public ServerKeepAlivePacket() { }
    }
}

