using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("SetBeaconEffect", PacketState.Play, PacketDirection.Serverbound)]
    public partial class SetBeaconEffectPacket : IClientPacket
    {
        [PacketSubInfo(393, 758)]
        public sealed partial class V393_758 : SetBeaconEffectPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, PrimaryEffect, SecondaryEffect);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int primaryEffect, int secondaryEffect)
            {
                writer.WriteVarInt(primaryEffect);
                writer.WriteVarInt(secondaryEffect);
            }

            public int PrimaryEffect { get; set; }
            public int SecondaryEffect { get; set; }
        }

        [PacketSubInfo(759, 769)]
        public sealed partial class V759_769 : SetBeaconEffectPacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, PrimaryEffect, SecondaryEffect);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                int? primaryEffect, int? secondaryEffect)
            {
                writer.WriteBoolean(primaryEffect is not null);
                if (primaryEffect is not null)
                    writer.WriteVarInt((int)primaryEffect);
                writer.WriteBoolean(secondaryEffect is not null);
                if (secondaryEffect is not null)
                    writer.WriteVarInt((int)secondaryEffect);
            }

            public int? PrimaryEffect { get; set; }
            public int? SecondaryEffect { get; set; }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V393_758.IsSupportedVersionStatic(protocolVersion))
                V393_758.SerializeInternal(ref writer, protocolVersion, 0, 0);
            else if (V759_769.IsSupportedVersionStatic(protocolVersion))
                V759_769.SerializeInternal(ref writer, protocolVersion, null, null);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.SetBeaconEffect), protocolVersion);
        }
    }
}