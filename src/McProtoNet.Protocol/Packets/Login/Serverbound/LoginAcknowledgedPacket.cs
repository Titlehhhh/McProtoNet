using McProtoNet.Serialization;

namespace McProtoNet.Protocol.ServerboundPackets.Login;

public class LoginAcknowledgedPacket : IClientPacket
{
    public static PacketIdentifier PacketId => ClientLoginPacket.LoginAcknowledged;

    public PacketIdentifier GetPacketId() => PacketId;


    public sealed class V340_769 : LoginAcknowledgedPacket
    {
        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 340 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V340_769.SupportedVersion(protocolVersion);
    }

    public void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
    }
}