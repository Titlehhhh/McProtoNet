using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;

namespace McProtoNet.PacketRepository754.Packets.Client
{
    [McProtoNet.API.Protocol.PacketInfo(0x02, 740, PacketCategory.Login, PacketSide.Client)]
    public class LoginPluginResponsePacket : IPacket
    {
        public int MessageID { get; set; }
        public byte[] Data { get; set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            throw new NotImplementedException();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
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
