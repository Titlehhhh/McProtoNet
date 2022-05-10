using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x2C, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerSwingArmPacket : IPacket
    {
        public Hand PlayerHand { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(PlayerHand);
        }
        public void Read(IMinecraftPrimitiveReader stream)
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
