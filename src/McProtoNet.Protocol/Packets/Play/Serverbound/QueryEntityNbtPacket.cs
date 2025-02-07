using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("QueryEntityNbt", PacketState.Play, PacketDirection.Serverbound)]
    public partial class QueryEntityNbtPacket : IClientPacket
    {
        public int TransactionId { get; set; }
        public int EntityId { get; set; }

        [PacketSubInfo(393, 769)]
        public sealed partial class V393_769 : QueryEntityNbtPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, TransactionId, EntityId);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int transactionId, int entityId)
            {
                writer.WriteVarInt(transactionId);
                writer.WriteVarInt(entityId);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.IsSupportedVersionStatic(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, TransactionId, EntityId);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.QueryEntityNbt), protocolVersion);
        }
    }
}