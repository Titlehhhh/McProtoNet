using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class RemoveResourcePackPacket : IServerPacket
{
    public sealed class V765_767 : RemoveResourcePackPacket
    {
        public Guid? UUID { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            UUID = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadUUID());
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 765 and <= 767;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V765_767.SupportedVersion(protocolVersion);
    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public Guid? UUID { get; set; }

    public static PacketIdentifier PacketId => ServerConfigurationPacket.RemoveResourcePack;

    public PacketIdentifier GetPacketId() => PacketId;
}