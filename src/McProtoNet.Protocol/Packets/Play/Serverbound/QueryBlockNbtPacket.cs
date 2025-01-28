using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("QueryBlockNbt", PacketState.Play, PacketDirection.Serverbound)]
    public partial class QueryBlockNbtPacket : IClientPacket
    {
        public int TransactionId { get; set; }
        public Position Location { get; set; }

        [PacketSubInfo(393, 769)]
        public sealed partial class V393_769 : QueryBlockNbtPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, TransactionId, Location);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int transactionId, Position location)
            {
                writer.WriteVarInt(transactionId);
                writer.WritePosition(location, protocolVersion);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_769.IsSupportedVersionStatic(protocolVersion))
                V393_769.SerializeInternal(ref writer, protocolVersion, TransactionId, Location);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.QueryBlockNbt), protocolVersion);
        }
    }
}