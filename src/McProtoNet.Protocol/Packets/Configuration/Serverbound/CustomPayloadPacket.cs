using McProtoNet.Serialization;
using McProtoNet.NBT;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

public class CustomPayloadPacket : IClientPacket
{
    public sealed class V764_769 : CustomPayloadPacket
    {
        public string Channel { get; set; }
        public byte[] Data { get; set; }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, string channel,
            byte[] data)
        {
            writer.WriteString(channel);
            writer.WriteBuffer(data);
        }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Channel, Data);
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
            V764_769.SerializeInternal(ref writer, protocolVersion, String.Empty, []);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.CustomPayload), protocolVersion);
    }

    public static PacketIdentifier PacketId => ClientConfigurationPacket.CustomPayload;

    public PacketIdentifier GetPacketId() => PacketId;
}