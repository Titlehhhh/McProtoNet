using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerUseItemPacket : IPacket
    {
        public Hand PlayerHand { get; private set; }
        public int MyProperty { get; private set; }
        public void Write(IMinecraftStreamWriter stream)
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

        public void Read(IMinecraftStreamReader stream)
        {

        }

        public ClientPlayerUseItemPacket(Hand hand, int myProperty)
        {
            PlayerHand = hand;
            MyProperty = myProperty;
        }
    }
}
