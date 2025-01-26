using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class BedPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public Position Location { get; set; }

        public sealed class V340_404 : BedPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 404;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_404.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.Bed;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}