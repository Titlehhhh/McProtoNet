namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientPlayerUseItemPacket : Packet
    {
        public Hand PlayerHand { get; private set; }
        public int MyProperty { get; private set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            switch (PlayerHand)
            {
                case Hand.MAINHAND:
                    stream.WriteVarInt(0);
                    break;
                case Hand.OFFHAND:
                    stream.WriteVarInt(1);
                    break;
            }
        }

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientPlayerUseItemPacket(Hand hand, int myProperty)
        {
            PlayerHand = hand;
            MyProperty = myProperty;
        }
    }
}
