using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class ResourcePackSendPacket : IServerPacket
{
    public sealed class V764 : ResourcePackSendPacket
    {
        public string Url { get; set; }
        public string Hash { get; set; }
        public bool Forced { get; set; }
        public string? PromptMessage { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Url = reader.ReadString();
            Hash = reader.ReadString();
            Forced = reader.ReadBoolean();
            PromptMessage = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadString());
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion == 764;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V764.SupportedVersion(protocolVersion);
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.ResourcePackSend;

    public PacketIdentifier GetPacketId() => PacketId;
}