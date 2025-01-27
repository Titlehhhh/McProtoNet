using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

public class KeepAlivePacket : IClientPacket
{
    public long KeepAliveId { get; set; }

    public sealed class V764_769 : KeepAlivePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, KeepAliveId);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            long keepAliveId)
        {
            writer.WriteSignedLong(keepAliveId);
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
        if (V764_769.SupportedVersion(protocolVersion))
            V764_769.SerializeInternal(ref writer, protocolVersion, KeepAliveId);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.KeepAlive), protocolVersion);
    }

    public static PacketIdentifier PacketId => ClientConfigurationPacket.KeepAlive;

    public PacketIdentifier GetPacketId() => PacketId;
}