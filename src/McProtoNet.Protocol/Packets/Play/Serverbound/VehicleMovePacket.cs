using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("VehicleMove", PacketState.Play, PacketDirection.Serverbound)]
    public partial class VehicleMovePacket : IClientPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        [PacketSubInfo(340, 768)]
        public sealed partial class V340_768 : VehicleMovePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x, double y, double z, float yaw, float pitch)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
            }
        }

        [PacketSubInfo(769, 769)]
        public sealed partial class V769 : VehicleMovePacket
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

            public bool OnGround { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_768.IsSupportedVersionStatic(protocolVersion))
                V340_768.SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch);
            else if (V769.IsSupportedVersionStatic(protocolVersion))
                V769.SerializeInternal(ref writer, protocolVersion, X, Y, Z, Yaw, Pitch, false);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.VehicleMove), protocolVersion);
        }
    }
}