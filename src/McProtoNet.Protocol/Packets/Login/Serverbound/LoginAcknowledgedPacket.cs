using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginAcknowledged", PacketState.Login, PacketDirection.Serverbound)]
public partial class LoginAcknowledgedPacket : IClientPacket
{
    [PacketSubInfo(340,769)]
    public sealed partial class V340_769 : LoginAcknowledgedPacket
    {
    }


    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
    }
}