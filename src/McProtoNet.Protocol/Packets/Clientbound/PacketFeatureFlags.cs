using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class FeatureFlagsPacket : IServerPacket
    {
        public string[] Features { get; set; }

        internal sealed class V761_763 : FeatureFlagsPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Features = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.String);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 761 and <= 763;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V761_763.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.FeatureFlags;

        public ServerPacket GetPacketId() => PacketId;
    }
}