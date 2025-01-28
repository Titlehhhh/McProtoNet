using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Transaction", PacketState.Play, PacketDirection.Serverbound)]
    public partial class TransactionPacket : IClientPacket
    {
        public sbyte WindowId { get; set; }
        public short Action { get; set; }
        public bool Accepted { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : TransactionPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, WindowId, Action, Accepted);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte windowId, short action, bool accepted)
            {
                writer.WriteSignedByte(windowId);
                writer.WriteSignedShort(action);
                writer.WriteBoolean(accepted);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_754.IsSupportedVersionStatic(protocolVersion))
                V340_754.SerializeInternal(ref writer, protocolVersion, WindowId, Action, Accepted);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Transaction), protocolVersion);
        }
    }
}