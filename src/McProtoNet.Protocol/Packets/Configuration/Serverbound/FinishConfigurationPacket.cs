using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("FinishConfiguration", PacketState.Configuration, PacketDirection.Serverbound)]
public partial class FinishConfigurationPacket : IClientPacket
{
    [PacketSubInfo(764,769)]
    public sealed partial class V764_769 : FinishConfigurationPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
        }

        
    }

    

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (!V764_769.IsSupportedVersionStatic(protocolVersion))
        {
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.FinishConfiguration),
                protocolVersion);
        }
    }
}