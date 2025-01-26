using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets.Play
{
    public class PositionLookPacket : IClientPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public sealed class V340_767 : PositionLookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, OnGround);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x, double y, double z, float yaw, float pitch, bool onGround)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
                writer.WriteBoolean(onGround);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 767;
            }

            public bool OnGround { get; set; }
        }

        public sealed class V768_769 : PositionLookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x, double y, double z, float yaw, float pitch, byte flags)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
                writer.WriteUnsignedByte(flags);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public byte Flags { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.SupportedVersion(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, false);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PositionLook), protocolVersion);
        }

        public static PacketIdentifier PacketId => ClientPlayPacket.PositionLook;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}