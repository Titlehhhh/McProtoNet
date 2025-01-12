using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class SteerVehiclePacket : IClientPacket
    {
        public float Sideways { get; set; }
        public float Forward { get; set; }
        public byte Jump { get; set; }

        internal sealed class V340_767 : SteerVehiclePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Sideways, Forward, Jump);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, float sideways, float forward, byte jump)
            {
                writer.WriteFloat(sideways);
                writer.WriteFloat(forward);
                writer.WriteUnsignedByte(jump);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 767;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_767.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.SupportedVersion(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, Sideways, Forward, Jump);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.SteerVehicle), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.SteerVehicle;

        public ClientPacket GetPacketId() => PacketId;
    }
}