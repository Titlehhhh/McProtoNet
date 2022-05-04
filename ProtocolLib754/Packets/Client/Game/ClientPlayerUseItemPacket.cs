using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x2F, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientPlayerUseItemPacket : IPacket
    {
        public Hand PlayerHand { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt(PlayerHand);
        }
        public void Read(IMinecraftStreamReader stream)
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
