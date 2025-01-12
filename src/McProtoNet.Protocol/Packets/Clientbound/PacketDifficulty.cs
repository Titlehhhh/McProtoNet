using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class DifficultyPacket : IServerPacket
    {
        public byte Difficulty { get; set; }

        internal sealed class V340_404 : DifficultyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Difficulty = reader.ReadUnsignedByte();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 404;
            }
        }

        internal sealed class V477_769 : DifficultyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Difficulty = reader.ReadUnsignedByte();
                DifficultyLocked = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 769;
            }

            public bool DifficultyLocked { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_404.SupportedVersion(protocolVersion) || V477_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.Difficulty;

        public ServerPacket GetPacketId() => PacketId;
    }
}