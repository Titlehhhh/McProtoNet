namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x2F, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPlayerUseItemPacket : Packet
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
        public ClientPlayerUseItemPacket() { }

        public ClientPlayerUseItemPacket(Hand PlayerHand)
        {
            this.PlayerHand = PlayerHand;
        }
    }
}
