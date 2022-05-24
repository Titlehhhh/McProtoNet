namespace McProtoNet.PacketRepository754.Packets.Server
{
    [PacketInfo(0x04, 754, PacketSide.Server)]
    public sealed class LoginPluginRequestPacket : Packet
    {
        public int MessageID { get; set; }
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {

            MessageID = stream.ReadVarInt();
            Channel = stream.ReadString();
            Data = stream.ReadToEnd();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public LoginPluginRequestPacket()
        {

        }
    }
}
