using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

public class ResourcePackReceivePacket : IClientPacket
{
    public int Result { get; set; }

    public Guid Uuid { get; set; }

    public sealed class V764 : ResourcePackReceivePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Result);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int result)
        {
            writer.WriteVarInt(result);
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion == 764;
        }
    }

    public sealed class V765_769 : ResourcePackReceivePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Uuid, Result);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Guid uuid,
            int result)
        {
            writer.WriteUUID(uuid);
            writer.WriteVarInt(result);
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

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764.SupportedVersion(protocolVersion))
            V764.SerializeInternal(ref writer, protocolVersion, Result);
        else if (V765_769.SupportedVersion(protocolVersion))
            V765_769.SerializeInternal(ref writer, protocolVersion, Uuid, Result);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.ResourcePackReceive),
                protocolVersion);
    }

    public static PacketIdentifier PacketId => ClientConfigurationPacket.ResourcePackReceive;

    public PacketIdentifier GetPacketId() => PacketId;
}