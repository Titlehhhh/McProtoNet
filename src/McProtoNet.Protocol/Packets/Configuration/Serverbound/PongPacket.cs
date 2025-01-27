using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

public class PongPacket : IClientPacket
{
    public int Id { get; set; }

    public sealed class V764_769 : PongPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Id);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int id)
        {
            writer.WriteSignedInt(id);
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
            V764_769.SerializeInternal(ref writer, protocolVersion, Id);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.Pong), protocolVersion);
    }

    public static PacketIdentifier PacketId => ClientConfigurationPacket.Pong;

    public PacketIdentifier GetPacketId() => PacketId;
}