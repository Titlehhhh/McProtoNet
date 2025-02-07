using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Flying", PacketState.Play, PacketDirection.Serverbound)]
    public partial class FlyingPacket : IClientPacket
    {
        [PacketSubInfo(340, 767)]
        public sealed partial class V340_767 : FlyingPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, OnGround);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                bool onGround)
            {
                writer.WriteBoolean(onGround);
            }

            public bool OnGround { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : FlyingPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, byte flags)
            {
                writer.WriteUnsignedByte(flags);
            }

            public byte Flags { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_767.IsSupportedVersionStatic(protocolVersion))
                V340_767.SerializeInternal(ref writer, protocolVersion, false);
            else if (V768_769.IsSupportedVersionStatic(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Flying), protocolVersion);
        }
    }
}