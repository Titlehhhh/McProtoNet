using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class SetBeaconEffectPacket : IClientPacket
    {
        public sealed class V393_758 : SetBeaconEffectPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, PrimaryEffect, SecondaryEffect);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int primaryEffect, int secondaryEffect)
            {
                writer.WriteVarInt(primaryEffect);
                writer.WriteVarInt(secondaryEffect);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 393 and <= 758;
            }

            public int PrimaryEffect { get; set; }
            public int SecondaryEffect { get; set; }
        }

        public sealed class V759_769 : SetBeaconEffectPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, PrimaryEffect, SecondaryEffect);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int? primaryEffect, int? secondaryEffect)
            {
                writer.WriteBoolean(primaryEffect is not null);
                if (primaryEffect is not null)
                writer.WriteVarInt((int)primaryEffect);
                writer.WriteBoolean(secondaryEffect is not null);
                if (secondaryEffect is not null)
                writer.WriteVarInt((int)secondaryEffect);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 769;
            }

            public int? PrimaryEffect { get; set; }
            public int? SecondaryEffect { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V393_758.SupportedVersion(protocolVersion) || V759_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_758.SupportedVersion(protocolVersion))
                V393_758.SerializeInternal(ref writer, protocolVersion, 0, 0);
            else if (V759_769.SupportedVersion(protocolVersion))
                V759_769.SerializeInternal(ref writer, protocolVersion, null, null);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.SetBeaconEffect), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.SetBeaconEffect;

        public ClientPacket GetPacketId() => PacketId;
    }
}