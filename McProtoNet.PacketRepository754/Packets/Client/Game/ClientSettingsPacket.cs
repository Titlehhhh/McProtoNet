namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x05, 754, PacketCategory.Game, PacketSide.Client)]
    public sealed class ClientSettingsPacket : Packet
    {


        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientSettingsPacket() { }


    }
}
