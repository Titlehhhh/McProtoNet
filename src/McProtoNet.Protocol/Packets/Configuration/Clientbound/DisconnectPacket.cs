using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class DisconnectPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.Disconnect;

    public PacketIdentifier GetPacketId() => PacketId;

    public sealed class V764 : DisconnectPacket
    {
        public string Reason { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Reason = reader.ReadString();
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 764 and <= 769;
        }
    }

    public sealed class V765_769 : DisconnectPacket
    {
        public NbtTag Reason { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Reason = reader.ReadNbtTag(readRootTag: false);
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 765 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V764.SupportedVersion(protocolVersion) || V765_769.SupportedVersion(protocolVersion);
    }
}