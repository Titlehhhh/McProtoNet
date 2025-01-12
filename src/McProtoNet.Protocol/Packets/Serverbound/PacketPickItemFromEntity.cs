using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class PickItemFromEntityPacket : IClientPacket
    {
        public int EntityId { get; set; }
        public bool IncludeData { get; set; }

        internal sealed class V769 : PickItemFromEntityPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, EntityId, IncludeData);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int entityId, bool includeData)
            {
                writer.WriteVarInt(entityId);
                writer.WriteBoolean(includeData);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V769.SupportedVersion(protocolVersion))
                V769.SerializeInternal(ref writer, protocolVersion, EntityId, IncludeData);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.PickItemFromEntity), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.PickItemFromEntity;

        public ClientPacket GetPacketId() => PacketId;
    }
}