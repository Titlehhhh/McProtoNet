namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0E, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerChatPacket : IPacket
    {
        public string Message { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Message);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Message = stream.ReadString();
        }
        public ServerChatPacket() { }
    }
}

