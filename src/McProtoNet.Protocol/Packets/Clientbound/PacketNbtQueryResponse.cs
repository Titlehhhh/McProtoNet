using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class NbtQueryResponsePacket : IServerPacket
    {
        public int TransactionId { get; set; }
        public NbtTag? Nbt { get; set; }

        internal sealed class V393_763 : NbtQueryResponsePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TransactionId = reader.ReadVarInt();
                Nbt = reader.ReadOptionalNbtTag(true);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 393 and <= 763;
            }
        }

        internal sealed class V764_769 : NbtQueryResponsePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TransactionId = reader.ReadVarInt();
                Nbt = reader.ReadOptionalNbtTag(false);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393_763.SupportedVersion(protocolVersion) || V764_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.NbtQueryResponse;

        public ServerPacket GetPacketId() => PacketId;
    }
}