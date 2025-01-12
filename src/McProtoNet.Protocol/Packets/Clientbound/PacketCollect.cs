using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class CollectPacket : IServerPacket
    {
        public int CollectedEntityId { get; set; }
        public int CollectorEntityId { get; set; }
        public int PickupItemCount { get; set; }

        internal sealed class V340_769 : CollectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                CollectedEntityId = reader.ReadVarInt();
                CollectorEntityId = reader.ReadVarInt();
                PickupItemCount = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.Collect;

        public ServerPacket GetPacketId() => PacketId;
    }
}