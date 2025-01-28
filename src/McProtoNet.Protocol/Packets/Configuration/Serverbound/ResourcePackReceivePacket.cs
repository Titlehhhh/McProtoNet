using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("ResourcePackReceive", PacketState.Configuration, PacketDirection.Serverbound)]
public partial class ResourcePackReceivePacket : IClientPacket
{
    public int Result { get; set; }

    public Guid Uuid { get; set; }  

    [PacketSubInfo(764,764)]
    public sealed partial class V764 : ResourcePackReceivePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Result);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, int result)
        {
            writer.WriteVarInt(result);
        }

    }


    [PacketSubInfo(765,769)]
    public sealed partial class V765_769 : ResourcePackReceivePacket
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

    }


    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764.IsSupportedVersionStatic(protocolVersion))
            V764.SerializeInternal(ref writer, protocolVersion, Result);
        else if (V765_769.IsSupportedVersionStatic(protocolVersion))
            V765_769.SerializeInternal(ref writer, protocolVersion, Uuid, Result);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.ResourcePackReceive),
                protocolVersion);
    }

}