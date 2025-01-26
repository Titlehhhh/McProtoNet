using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class EntityEffectPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int Duration { get; set; }

        public sealed class V340_757 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadSignedByte();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 757;
            }

            public sbyte EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
        }

        public sealed class V758 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 758;
            }

            public int EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
        }

        public sealed class V759_763 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
                FactorCodec = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadNbtTag(true));
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 763;
            }

            public int EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
            public NbtTag? FactorCodec { get; set; }
        }

        public sealed class V764_765 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
                FactorCodec = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadNbtTag(false));
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 765;
            }

            public int EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
            public NbtTag? FactorCodec { get; set; }
        }

        public sealed class V766_769 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadVarInt();
                Duration = reader.ReadVarInt();
                Flags = reader.ReadUnsignedByte();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 766 and <= 769;
            }

            public int EffectId { get; set; }
            public int Amplifier { get; set; }
            public byte Flags { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_757.SupportedVersion(protocolVersion) || V758.SupportedVersion(protocolVersion) || V759_763.SupportedVersion(protocolVersion) || V764_765.SupportedVersion(protocolVersion) || V766_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.EntityEffect;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}