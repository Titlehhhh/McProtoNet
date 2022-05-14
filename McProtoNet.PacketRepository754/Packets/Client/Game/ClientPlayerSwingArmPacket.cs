namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x2C, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientPlayerSwingArmPacket : Packet
    {
        public Hand PlayerHand { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(PlayerHand);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            PlayerHand = (Hand)stream.ReadVarInt();
        }
        public ClientPlayerSwingArmPacket() { }

        public ClientPlayerSwingArmPacket(Hand PlayerHand)
        {
            this.PlayerHand = PlayerHand;
        }
    }
}
