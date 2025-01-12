using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class DebugSamplePacket : IServerPacket
    {
        public long[] Sample { get; set; }
        public int Type { get; set; }

        internal sealed class V766_769 : DebugSamplePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Sample = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.Int64);
                Type = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 766 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V766_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.DebugSample;

        public ServerPacket GetPacketId() => PacketId;
    }
}