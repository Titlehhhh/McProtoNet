using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class ExperiencePacket : IServerPacket
    {
        public float ExperienceBar { get; set; }
        public int Level { get; set; }
        public int TotalExperience { get; set; }

        internal sealed class V340_760 : ExperiencePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ExperienceBar = reader.ReadFloat();
                Level = reader.ReadVarInt();
                TotalExperience = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 760;
            }
        }

        internal sealed class V761_763 : ExperiencePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ExperienceBar = reader.ReadFloat();
                TotalExperience = reader.ReadVarInt();
                Level = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 761 and <= 763;
            }
        }

        internal sealed class V764_769 : ExperiencePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ExperienceBar = reader.ReadFloat();
                Level = reader.ReadVarInt();
                TotalExperience = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_760.SupportedVersion(protocolVersion) || V761_763.SupportedVersion(protocolVersion) ||
                   V764_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.Experience;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}