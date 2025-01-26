using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class StartConfigurationPacket : IServerPacket
    {
        public sealed class V764_769 : StartConfigurationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 764 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V764_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.StartConfiguration;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}