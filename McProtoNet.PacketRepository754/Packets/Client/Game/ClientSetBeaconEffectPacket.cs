using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x24, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientSetBeaconEffectPacket : IPacket
    {
        public int PrimaryEffect { get; private set; }
        public int SecondaryEffect { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(PrimaryEffect);
            stream.WriteVarInt(SecondaryEffect);
        }
        public void Read(IMinecraftPrimitiveReader stream)
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
