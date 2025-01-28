using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("FinishConfiguration", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class FinishConfigurationPacket : IServerPacket
{
    [PacketSubInfo(764,769)]
    public sealed partial class V764_769 : FinishConfigurationPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
        }

       
    }


    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

}