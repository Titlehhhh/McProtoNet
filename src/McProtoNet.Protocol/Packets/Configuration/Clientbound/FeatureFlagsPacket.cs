using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class FeatureFlagsPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.FeatureFlags;

    public PacketIdentifier GetPacketId() => PacketId;

    public sealed class V764_769 : FeatureFlagsPacket
    {
        public string[] Features { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            var count = reader.ReadVarInt();
            Features = new string[count];
            for (var i = 0; i < count; i++)
            {
                Features[i] = reader.ReadString();
            }
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
}