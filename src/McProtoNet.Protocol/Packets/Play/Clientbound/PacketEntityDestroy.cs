using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class EntityDestroyPacket : IServerPacket
    {
        public int[] EntityIds { get; set; }

        public sealed class V340_754 : EntityDestroyPacket
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

        public sealed class V756_769 : EntityDestroyPacket
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
        public static PacketIdentifier PacketId => ServerPlayPacket.EntityDestroy;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}