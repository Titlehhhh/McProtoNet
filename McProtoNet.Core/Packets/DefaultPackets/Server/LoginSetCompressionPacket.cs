using McProtoNet.Core.IO;
using McProtoNet.Core.Protocol;

namespace McProtoNet.Core.Packets.DefaultPackets.Server
{

    public sealed class LoginSetCompressionPacket : IMinecraftPacket
    {
        public int Threshold { get; set; }

        public void Read(IMinecraftPrimitiveReader stream)
        {
            Threshold = stream.ReadVarInt();
        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Threshold);
        }

        public LoginSetCompressionPacket(int threshold)
        {
            Threshold = threshold;
        }

        public LoginSetCompressionPacket()
        {

        }
    }
}
