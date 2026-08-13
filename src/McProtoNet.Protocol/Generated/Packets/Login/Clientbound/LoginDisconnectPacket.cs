using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.disconnect", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("Reason", "string")]
public sealed partial record LoginDisconnectPacket(string Reason) : IPacket<LoginDisconnectPacket>, IPacket
{
    public static LoginDisconnectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginDisconnectPacket>(protocolVersion);
        var reason = reader.ReadString();
        return new LoginDisconnectPacket(reason);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginDisconnectPacket>(protocolVersion);
        writer.WriteString(Reason);
    }

    public static PacketIdentity Identity => new("login.toClient.disconnect", "LoginDisconnect", PacketPhase.Login, PacketDirection.Clientbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
        {
            id = 0x00;
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
