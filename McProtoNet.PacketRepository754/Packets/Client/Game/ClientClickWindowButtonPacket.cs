namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x08, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientClickWindowButtonPacket : IPacket
    {
        public byte WindowId { get; private set; }
        public byte ButtonId { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteUnsignedByte(WindowId);
            stream.WriteUnsignedByte(ButtonId);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            WindowId = stream.ReadUnsignedByte();
            ButtonId = stream.ReadUnsignedByte();
        }
        public ClientClickWindowButtonPacket() { }

        public ClientClickWindowButtonPacket(byte WindowId, byte ButtonId)
        {
            this.WindowId = WindowId;
            this.ButtonId = ButtonId;
        }
    }
}
