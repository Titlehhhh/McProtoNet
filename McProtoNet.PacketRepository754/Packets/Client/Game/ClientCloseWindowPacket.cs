namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x0A, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientCloseWindowPacket : IPacket
    {
        public byte WindowId { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteUnsignedByte(WindowId);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            WindowId = stream.ReadUnsignedByte();
        }
        public ClientCloseWindowPacket() { }

        public ClientCloseWindowPacket(byte WindowId)
        {
            this.WindowId = WindowId;
        }
    }
}
