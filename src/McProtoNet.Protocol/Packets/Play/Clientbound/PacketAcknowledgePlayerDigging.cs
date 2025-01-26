using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class AcknowledgePlayerDiggingPacket : IServerPacket
    {
        public sealed class V498_758 : AcknowledgePlayerDiggingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Block = reader.ReadVarInt();
                Status = reader.ReadVarInt();
                Successful = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 498 and <= 758;
            }

            public Position Location { get; set; }
            public int Block { get; set; }
            public int Status { get; set; }
            public bool Successful { get; set; }
        }

        public sealed class V759_769 : AcknowledgePlayerDiggingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                SequenceId = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 769;
            }

            public int SequenceId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V498_758.SupportedVersion(protocolVersion) || V759_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.AcknowledgePlayerDigging;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}