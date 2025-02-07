using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("EntityAction", PacketState.Play, PacketDirection.Serverbound)]
    public partial class EntityActionPacket : IClientPacket
    {
        public int EntityId { get; set; }
        public int ActionId { get; set; }
        public int JumpBoost { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : EntityActionPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, EntityId, ActionId, JumpBoost);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int entityId, int actionId, int jumpBoost)
            {
                writer.WriteVarInt(entityId);
                writer.WriteVarInt(actionId);
                writer.WriteVarInt(jumpBoost);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_769.IsSupportedVersionStatic(protocolVersion))
                V340_769.SerializeInternal(ref writer, protocolVersion, EntityId, ActionId, JumpBoost);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.EntityAction), protocolVersion);
        }
    }
}