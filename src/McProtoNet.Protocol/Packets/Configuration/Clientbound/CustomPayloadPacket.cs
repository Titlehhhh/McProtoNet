using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class CustomPayloadPacket : IServerPacket
{
    public sealed class V764_769 : AddResourcePackPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Channel = reader.ReadString();
            Data = reader.ReadRestBuffer();
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

    public static PacketIdentifier PacketId => ServerConfigurationPacket.CustomPayload;

    public PacketIdentifier GetPacketId() => PacketId;
}