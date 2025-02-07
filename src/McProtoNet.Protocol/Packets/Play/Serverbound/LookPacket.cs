using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Look", PacketState.Play, PacketDirection.Serverbound)]
    public partial class LookPacket : IClientPacket
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : LookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Yaw, Pitch, OnGround);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, float yaw,
                float pitch, bool onGround)
            {
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
                writer.WriteBoolean(onGround);
            }

            public bool OnGround { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : LookPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Yaw, Pitch, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, float yaw,
                float pitch, byte flags)
            {
                writer.WriteFloat(yaw);
                writer.WriteFloat(pitch);
                writer.WriteUnsignedByte(flags);
            }

            public byte Flags { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, Yaw, Pitch, false);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, Yaw, Pitch, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Look), protocolVersion);
        }
    }
}