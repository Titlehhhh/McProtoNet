using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class ResetChatPacket : IServerPacket
{
    public sealed class V766_769 : ResetChatPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 766 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V766_769.SupportedVersion(protocolVersion);
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.ResetChat;

    public PacketIdentifier GetPacketId() => PacketId;
}