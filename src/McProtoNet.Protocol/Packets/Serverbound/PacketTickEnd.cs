using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class TickEndPacket : IClientPacket
    {
        internal sealed class V768_769 : TickEndPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.TickEnd), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.TickEnd;

        public ClientPacket GetPacketId() => PacketId;
    }
}