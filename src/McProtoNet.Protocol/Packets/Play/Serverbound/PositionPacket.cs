using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Position", PacketState.Play, PacketDirection.Serverbound)]
    public partial class PositionPacket : IClientPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : PositionPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, OnGround);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x,
                double y, double z, bool onGround)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteBoolean(onGround);
            }

            public bool OnGround { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : PositionPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, X, Y, Z, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, double x,
                double y, double z, byte flags)
            {
                writer.WriteDouble(x);
                writer.WriteDouble(y);
                writer.WriteDouble(z);
                writer.WriteUnsignedByte(flags);
            }

            public byte Flags { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, X, Y, Z, false);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, X, Y, Z, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Position), protocolVersion);
        }
    }
}