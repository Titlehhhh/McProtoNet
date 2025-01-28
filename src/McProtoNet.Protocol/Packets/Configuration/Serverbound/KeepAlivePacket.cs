using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("KeepAlive", PacketState.Configuration, PacketDirection.Serverbound)]
public partial class KeepAlivePacket : IClientPacket
{
    public long KeepAliveId { get; set; }   

    [PacketSubInfo(764,769)]
    public sealed partial class V764_769 : KeepAlivePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, KeepAliveId);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            long keepAliveId)
        {
            writer.WriteSignedLong(keepAliveId);
        }

    }

    

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764_769.IsSupportedVersionStatic(protocolVersion))
            V764_769.SerializeInternal(ref writer, protocolVersion, KeepAliveId);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.KeepAlive), protocolVersion);
    }

}