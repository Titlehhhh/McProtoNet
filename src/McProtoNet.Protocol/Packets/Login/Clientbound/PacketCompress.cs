using McProtoNet.Serialization;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.ClientboundPackets.Login
{
    public sealed class CompressPacket : IServerPacket
    {
        public int Threshold { get; set; }


        public void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Threshold = reader.ReadVarInt();
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 340 and <= 769;
        }

        public static PacketIdentifier PacketId => ServerLoginPacket.Compress;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}