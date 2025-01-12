using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class AbilitiesPacket : IClientPacket
    {
        public sbyte Flags { get; set; }

        internal sealed class V340_710 : AbilitiesPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Flags, FlyingSpeed, WalkingSpeed);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte flags, float flyingSpeed, float walkingSpeed)
            {
                writer.WriteSignedByte(flags);
                writer.WriteFloat(flyingSpeed);
                writer.WriteFloat(walkingSpeed);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 710;
            }

            public float FlyingSpeed { get; set; }
            public float WalkingSpeed { get; set; }
        }

        internal sealed class V734_769 : AbilitiesPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte flags)
            {
                writer.WriteSignedByte(flags);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 734 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_710.SupportedVersion(protocolVersion) || V734_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_710.SupportedVersion(protocolVersion))
                V340_710.SerializeInternal(ref writer, protocolVersion, Flags, 0, 0);
            else if (V734_769.SupportedVersion(protocolVersion))
                V734_769.SerializeInternal(ref writer, protocolVersion, Flags);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.Abilities), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.Abilities;

        public ClientPacket GetPacketId() => PacketId;
    }
}