using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
{

    [PacketInfo(0x24, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientSetBeaconEffectPacket : IPacket
    {
        public int PrimaryEffect { get; private set; }
        public int SecondaryEffect { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteVarInt(PrimaryEffect);
            stream.WriteVarInt(SecondaryEffect);
        }
        public void Read(IMinecraftStreamReader stream)
        {
            PrimaryEffect = stream.ReadVarInt();
            SecondaryEffect = stream.ReadVarInt();
        }
        public ClientSetBeaconEffectPacket() { }

        public ClientSetBeaconEffectPacket(int PrimaryEffect, int SecondaryEffect)
        {
            this.PrimaryEffect = PrimaryEffect;
            this.SecondaryEffect = SecondaryEffect;
        }
    }
}
