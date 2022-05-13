namespace McProtoNet.PacketRepository754.Packets.Server
{

    [PacketInfo(0x2D, 754, PacketCategory.Game, PacketSide.Server)]
    public class ServerOpenWindowPacket : IPacket
    {
        public int Id { get; set; }
        public WindowType WinType { get; set; }
        public string Name { get; set; }
        public void Write(IMinecraftPrimitiveWriter stream)
        {
            throw new NotImplementedException();
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Id = stream.ReadVarInt();
            WinType = (WindowType)stream.ReadVarInt();
            Name = stream.ReadString();
        }
        public ServerOpenWindowPacket() { }
    }
}

