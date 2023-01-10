namespace McProtoNet.Protocol754.Packets.Server
{


    public sealed class ServerDisconnectPacket : MinecraftPacket<Protocol754>
    {
        public string Message { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Message);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }
        public ServerDisconnectPacket() { }
    }
}

