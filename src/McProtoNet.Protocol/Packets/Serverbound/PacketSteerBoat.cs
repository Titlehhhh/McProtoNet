using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class SteerBoatPacket : IClientPacket
    {
        public bool LeftPaddle { get; set; }
        public bool RightPaddle { get; set; }

        internal sealed class V340_769 : SteerBoatPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, LeftPaddle, RightPaddle);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, bool leftPaddle, bool rightPaddle)
            {
                writer.WriteBoolean(leftPaddle);
                writer.WriteBoolean(rightPaddle);
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
                V340_769.SerializeInternal(ref writer, protocolVersion, LeftPaddle, RightPaddle);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.SteerBoat), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.SteerBoat;

        public ClientPacket GetPacketId() => PacketId;
    }
}