using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("PositionLook", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PositionLookPacket : IClientPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : PositionLookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, OnGround);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x,
                double y, double z, float yaw, float pitch, bool onGround)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
                writer.WriteBoolean(onGround);
            }

            public bool OnGround { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : PositionLookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x,
                double y, double z, float yaw, float pitch, byte flags)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
                writer.WriteUnsignedByte(flags);
            }

            public byte Flags { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, false);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.PositionLook), protocolVersion);
        }
    }
}