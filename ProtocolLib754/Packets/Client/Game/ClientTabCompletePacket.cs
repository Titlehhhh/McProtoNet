using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x06, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientTabCompletePacket : IPacket
    {
        public int TransactionId { get; private set; }
        public string Text { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt(TransactionId);
            stream.WriteString(Text);
        }
        public void Read(IMinecraftStreamReader stream)
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
