using McProtoNet.Serialization;
using McProtoNet.NBT;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("CustomPayload", PacketState.Configuration, PacketDirection.Serverbound)]
public partial class CustomPayloadPacket : IClientPacket
{
    [PacketSubInfo(764, 769)]
    public sealed partial class V764_769 : CustomPayloadPacket
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
    }


    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764_769.IsSupportedVersionStatic(protocolVersion))
            V764_769.SerializeInternal(ref writer, protocolVersion, String.Empty, []);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.CustomPayload), protocolVersion);
    }
}