using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

public class FinishConfigurationPacket : IClientPacket
{
    public sealed class V764_769 : FinishConfigurationPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
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

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (!V764_769.SupportedVersion(protocolVersion))
        {
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.FinishConfiguration),
                protocolVersion);
        }
    }

    public static PacketIdentifier PacketId => ClientConfigurationPacket.FinishConfiguration;

    public PacketIdentifier GetPacketId() => PacketId;
}