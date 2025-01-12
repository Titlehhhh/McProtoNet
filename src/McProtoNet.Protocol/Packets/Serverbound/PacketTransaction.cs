using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class TransactionPacket : IClientPacket
    {
        public sbyte WindowId { get; set; }
        public short Action { get; set; }
        public bool Accepted { get; set; }

        internal sealed class V340_754 : TransactionPacket
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

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 754;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_754.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_754.SupportedVersion(protocolVersion))
                V340_754.SerializeInternal(ref writer, protocolVersion, WindowId, Action, Accepted);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.Transaction), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.Transaction;

        public ClientPacket GetPacketId() => PacketId;
    }
}