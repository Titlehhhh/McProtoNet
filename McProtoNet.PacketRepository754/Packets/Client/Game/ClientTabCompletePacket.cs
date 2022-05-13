namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x06, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientTabCompletePacket : IPacket
    {
        public int TransactionId { get; private set; }
        public string Text { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(TransactionId);
            stream.WriteString(Text);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            TransactionId = stream.ReadVarInt();
            Text = stream.ReadString();
        }
        public ClientTabCompletePacket() { }

        public ClientTabCompletePacket(int TransactionId, string Text)
        {
            this.TransactionId = TransactionId;
            this.Text = Text;
        }
    }
}
