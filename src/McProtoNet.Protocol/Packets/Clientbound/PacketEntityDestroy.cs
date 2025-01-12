using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class EntityDestroyPacket : IServerPacket
    {
        public int[] EntityIds { get; set; }

        internal sealed class V340_754 : EntityDestroyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityIds = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 754;
            }
        }

        internal sealed class V756_769 : EntityDestroyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityIds = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 756 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_754.SupportedVersion(protocolVersion) || V756_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.EntityDestroy;

        public ServerPacket GetPacketId() => PacketId;
    }
}