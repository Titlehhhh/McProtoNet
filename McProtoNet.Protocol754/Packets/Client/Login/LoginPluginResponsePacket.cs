namespace McProtoNet.Protocol754.Packets.Client
{
    [PacketInfo(0x02, PacketCategory.Login, 740, PacketSide.Client)]
    public sealed class LoginPluginResponsePacket : MinecraftPacket
    {
        public int MessageID { get; set; }
        public byte[] Data { get; set; }

        public override void Read(IMinecraftPrimitiveReader stream)
        {
            throw new NotImplementedException();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(MessageID);
            if (Data != null)
            {
                stream.WriteBoolean(true);
                stream.Write(Data);
            }
            else
            {
                stream.WriteBoolean(false);
            }
        }

        public LoginPluginResponsePacket(int messageID, byte[] data)
        {
            MessageID = messageID;
            Data = data;
        }
        public LoginPluginResponsePacket()
        {

        }
    }
}
