using McProtoNet.Protocol754.Data.Entity.Player;
using McProtoNet.Protocol754.Data.Setting;

namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x05, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientSettingsPacket : Packet
    {
        public string Locale { get; set; }
        public byte RenderDistance { get; set; }
        public ChatVisibility ChatVisibility { get; set; }
        public bool UseChatColors { get; set; }
        public List<SkinPart> VisibleParts { get; set; }
        public HandPreference MainHand { get; set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Locale);
            stream.WriteUnsignedByte(RenderDistance);
            stream.WriteVarInt(ChatVisibility);
            stream.WriteBoolean(UseChatColors);

            int flags = 0;
            foreach (SkinPart part in VisibleParts)
            {
                flags |= 1 << ((int)part);
            }

            stream.WriteUnsignedByte((byte)flags);

            stream.WriteVarInt(MainHand);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSettingsPacket() { }


    }
}
