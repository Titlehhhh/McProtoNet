namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x0E, 754, PacketCategory.Game, PacketSide.Server)]
    public sealed class ServerChatPacket : Packet
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
        public ServerChatPacket() { }
    }
}

