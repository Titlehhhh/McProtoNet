using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ResetChat", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class ResetChatPacket : IServerPacket
{
    [PacketSubInfo(766,769)]
    public sealed partial class V766_769 : ResetChatPacket
    {
        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
        }

    }

    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

}