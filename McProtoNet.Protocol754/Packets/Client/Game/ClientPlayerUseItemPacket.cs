namespace McProtoNet.Protocol754.Packets.Client
{


    public sealed class ClientPlayerUseItemPacket : MinecraftPacket<Protocol754>
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
