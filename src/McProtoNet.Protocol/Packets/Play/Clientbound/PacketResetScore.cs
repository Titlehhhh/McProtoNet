using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class ResetScorePacket : IServerPacket
    {
        public string EntityName { get; set; }
        public string? ObjectiveName { get; set; }

        public sealed class V765_769 : ResetScorePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityName = reader.ReadString();
                ObjectiveName = reader.ReadOptional(ReadDelegates.String);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V765_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.ResetScore;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}