using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class FinishConfigurationPacket : IServerPacket
{
    public sealed class V764_769 : FinishConfigurationPacket
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

    public static PacketIdentifier PacketId => ServerConfigurationPacket.FinishConfiguration;

    public PacketIdentifier GetPacketId() => PacketId;
}