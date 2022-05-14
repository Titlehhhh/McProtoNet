namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x2D, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientSpectatePacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSpectatePacket() { }


    }
}
