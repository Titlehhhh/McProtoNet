using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class ScoreboardDisplayObjectivePacket : IServerPacket
    {
        public string Name { get; set; }

        public sealed class V340_763 : ScoreboardDisplayObjectivePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Position = reader.ReadSignedByte();
                Name = reader.ReadString();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 763;
            }

            public sbyte Position { get; set; }
        }

        public sealed class V764_769 : ScoreboardDisplayObjectivePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Position = reader.ReadVarInt();
                Name = reader.ReadString();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }

            public int Position { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_763.SupportedVersion(protocolVersion) || V764_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.ScoreboardDisplayObjective;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}