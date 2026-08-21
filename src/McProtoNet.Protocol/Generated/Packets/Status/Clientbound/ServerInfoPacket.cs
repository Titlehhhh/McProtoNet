using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Status.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("status.toClient.server_info", PacketPhase.Status, PacketDirection.Clientbound)]
[PacketField("Response", "string")]
public sealed partial record ServerInfoPacket(string Response) : IPacket<ServerInfoPacket>, IPacket
{
    public static ServerInfoPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerInfoPacket>(protocolVersion);
        var response = reader.ReadString();
        return new ServerInfoPacket(response);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerInfoPacket>(protocolVersion);
        writer.WriteString(Response);
    }

    public static PacketIdentity Identity => new("status.toClient.server_info", "ServerInfo", PacketPhase.Status, PacketDirection.Clientbound, 1);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 776)
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
