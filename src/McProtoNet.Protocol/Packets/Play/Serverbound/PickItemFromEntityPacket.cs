using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("PickItemFromEntity", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PickItemFromEntityPacket : IClientPacket
    {
        public int EntityId { get; set; }
        public bool IncludeData { get; set; }

        [PacketSubInfo(769, 769)]
        public sealed partial class V769 : PickItemFromEntityPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, EntityId, IncludeData);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int entityId, bool includeData)
            {
                writer.WriteVarInt(entityId);
                writer.WriteBoolean(includeData);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V769.IsSupportedVersionStatic(protocolVersion))
                V769.SerializeInternal(ref writer, protocolVersion, EntityId, IncludeData);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PickItemFromEntity), protocolVersion);
        }
    }
}