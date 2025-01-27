using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class PingPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.Ping;

    public PacketIdentifier GetPacketId() => PacketId;

    public sealed class V764_769 : PingPacket
    {
        public int Id { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Id = reader.ReadSignedInt();
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