namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x2D, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerOpenWindowPacket : Packet
    {
        public int Id { get; set; }
        public WindowType WinType { get; set; }
        public string Name { get; set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            throw new NotImplementedException();
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Id = stream.ReadVarInt();
            WinType = (WindowType)stream.ReadVarInt();
            Name = stream.ReadString();
        }
        public ServerOpenWindowPacket() { }
    }
}

