using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class OpenSignEntityPacket : IServerPacket
    {
        public Position Location { get; set; }

        public sealed class V340_762 : OpenSignEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 762;
            }
        }

        public sealed class V763_769 : OpenSignEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                IsFrontText = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 763 and <= 769;
            }

            public bool IsFrontText { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_762.SupportedVersion(protocolVersion) || V763_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.OpenSignEntity;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}