namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x07, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientConfirmTransactionPacket : Packet
    {
        public byte WindowId { get; private set; }
        public ushort ActionId { get; private set; }
        public bool Accepted { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteUnsignedByte(WindowId);
            stream.WriteUnsignedLong(ActionId);
            stream.WriteBoolean(Accepted);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            WindowId = stream.ReadUnsignedByte();
            ActionId = stream.ReadUnsignedShort();
            Accepted = stream.ReadBoolean();
        }
        public ClientConfirmTransactionPacket() { }

        public ClientConfirmTransactionPacket(byte WindowId, ushort ActionId, bool Accepted)
        {
            this.WindowId = WindowId;
            this.ActionId = ActionId;
            this.Accepted = Accepted;
        }
    }
}
