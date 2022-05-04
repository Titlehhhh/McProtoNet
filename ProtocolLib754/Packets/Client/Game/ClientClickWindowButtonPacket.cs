using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x08, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientClickWindowButtonPacket : IPacket
    {
        public byte WindowId { get; private set; }
        public byte ButtonId { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteUnsignedByte(WindowId);
            stream.WriteUnsignedByte(ButtonId);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            WindowId = stream.ReadUnsignedByte();
            ButtonId = stream.ReadUnsignedByte();
        }
        public ClientClickWindowButtonPacket() { }

        public ClientClickWindowButtonPacket(byte WindowId, byte ButtonId)
        {
            this.WindowId = WindowId;
            this.ButtonId = ButtonId;
        }
    }
}
