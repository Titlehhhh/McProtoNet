using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class FlyingPacket : IClientPacket
    {
        public sealed class V340_767 : FlyingPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, OnGround);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, bool onGround)
            {
                writer.WriteBoolean(onGround);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 767;
            }

            public bool OnGround { get; set; }
        }

        public sealed class V768_769 : FlyingPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, byte flags)
            {
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
                V340_767.SerializeInternal(ref writer, protocolVersion, default);
            else if (V768_769.SupportedVersion(protocolVersion))
                V768_769.SerializeInternal(ref writer, protocolVersion, default);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.Flying), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.Flying;

        public ClientPacket GetPacketId() => PacketId;
    }
}