using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class EntityPacket : IServerPacket
    {
        public int EntityId { get; set; }

        internal sealed class V340_754 : EntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 754;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_754.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.Entity;

        public ServerPacket GetPacketId() => PacketId;
    }
}