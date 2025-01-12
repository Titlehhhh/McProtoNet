using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class ArmAnimationPacket : IClientPacket
    {
        public int Hand { get; set; }

        internal sealed class V340_769 : ArmAnimationPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Hand);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int hand)
            {
                writer.WriteVarInt(hand);
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
                V340_769.SerializeInternal(ref writer, protocolVersion, Hand);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.ArmAnimation), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.ArmAnimation;

        public ClientPacket GetPacketId() => PacketId;
    }
}