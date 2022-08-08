namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x0B, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPluginMessagePacket : Packet
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Channel);
            stream.Write(Data);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }
        public ClientPluginMessagePacket() { }


    }
}
