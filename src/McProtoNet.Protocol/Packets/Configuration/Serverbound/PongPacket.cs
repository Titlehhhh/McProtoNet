using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("Pong", PacketState.Configuration, PacketDirection.Serverbound)]
public partial class PongPacket : IClientPacket
{
    public int Id { get; set; }

    [PacketSubInfo(764, 769)]
    public sealed partial class V764_769 : PongPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Id);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int id)
        {
            writer.WriteSignedInt(id);
        }
    }


    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764_769.IsSupportedVersionStatic(protocolVersion))
            V764_769.SerializeInternal(ref writer, protocolVersion, Id);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.Pong), protocolVersion);
    }
}