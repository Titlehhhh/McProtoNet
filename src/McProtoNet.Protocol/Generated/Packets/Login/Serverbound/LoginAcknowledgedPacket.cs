using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.login_acknowledged", PacketPhase.Login, PacketDirection.Serverbound)]
public sealed partial record LoginAcknowledgedPacket() : IPacket<LoginAcknowledgedPacket>, IPacket
{
    public static LoginAcknowledgedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginAcknowledgedPacket>(protocolVersion);
        return new LoginAcknowledgedPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginAcknowledgedPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("login.toServer.login_acknowledged", "LoginAcknowledged", PacketPhase.Login, PacketDirection.Serverbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 772)
        {
            id = 0x03;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
