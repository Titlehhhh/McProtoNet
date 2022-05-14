﻿namespace McProtoNet.PacketRepository754.Packets.Server
{
    [PacketInfo(0x04, 754, PacketCategory.Login, PacketSide.Server)]
    public class LoginPluginRequestPacket : IPacket
    {
        public int MessageID { get; set; }
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            
            MessageID = stream.ReadVarInt();
            Channel = stream.ReadString();
            Data = stream.ReadToEnd();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public LoginPluginRequestPacket()
        {

        }
    }
}
