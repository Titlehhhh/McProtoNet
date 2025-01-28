using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("Abilities", PacketState.Play, PacketDirection.Serverbound)]
    public partial class AbilitiesPacket : IClientPacket
    {
        public sbyte Flags { get; set; }

        [PacketSubInfo(340, 710)]
        public sealed partial class V340_710 : AbilitiesPacket
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

            public float FlyingSpeed { get; set; }
            public float WalkingSpeed { get; set; }
        }

        [PacketSubInfo(734, 769)]
        public sealed partial class V734_769 : AbilitiesPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Flags);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, sbyte flags)
            {
                writer.WriteSignedByte(flags);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V340_710.IsSupportedVersionStatic(protocolVersion))
                V340_710.SerializeInternal(ref writer, protocolVersion, Flags, 0, 0);
            else if (V734_769.IsSupportedVersionStatic(protocolVersion))
                V734_769.SerializeInternal(ref writer, protocolVersion, Flags);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Abilities), protocolVersion);
        }
    }
}