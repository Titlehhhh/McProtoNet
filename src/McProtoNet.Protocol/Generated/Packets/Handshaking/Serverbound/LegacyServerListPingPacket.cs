using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("handshaking.toServer.legacy_server_list_ping", PacketPhase.Handshaking, PacketDirection.Serverbound)]
[PacketField("Payload", "int")]
public sealed partial record LegacyServerListPingPacket(int Payload) : IPacket<LegacyServerListPingPacket>, IPacket
{
    public static LegacyServerListPingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LegacyServerListPingPacket>(protocolVersion);
        var payload = reader.ReadUnsignedByte();
        return new LegacyServerListPingPacket(payload);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LegacyServerListPingPacket>(protocolVersion);
        writer.WriteUnsignedByte((byte)Payload);
    }

    public static PacketIdentity Identity => new("handshaking.toServer.legacy_server_list_ping", "LegacyServerListPing", PacketPhase.Handshaking, PacketDirection.Serverbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 776)
        {
            id = 0xFE;
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
