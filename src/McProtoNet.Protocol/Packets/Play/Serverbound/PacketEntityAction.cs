using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class EntityActionPacket : IClientPacket
    {
        public int EntityId { get; set; }
        public int ActionId { get; set; }
        public int JumpBoost { get; set; }

        internal sealed class V340_769 : EntityActionPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, EntityId, ActionId, JumpBoost);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int entityId, int actionId, int jumpBoost)
            {
                writer.WriteVarInt(entityId);
                writer.WriteVarInt(actionId);
                writer.WriteVarInt(jumpBoost);
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

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.SupportedVersion(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, EntityId, ActionId, JumpBoost);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.EntityAction), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.EntityAction;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}