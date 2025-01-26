using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class KickDisconnectPacket : IServerPacket
    {
        public sealed class V340_764 : KickDisconnectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reason = reader.ReadString();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 764;
            }

            public string Reason { get; set; }
        }

        public sealed class V765_769 : KickDisconnectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reason = reader.ReadNbtTag(false);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 769;
            }

            public NbtTag Reason { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_764.SupportedVersion(protocolVersion) || V765_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.KickDisconnect;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}