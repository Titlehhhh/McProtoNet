using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.ping", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Id", "int")]
public sealed partial record PingPacket(int Id) : IPacket<PingPacket>, IPacket
{
    public static PingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingPacket>(protocolVersion);
        var id = reader.ReadSignedInt();
        return new PingPacket(id);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingPacket>(protocolVersion);
        writer.WriteSignedInt(Id);
    }

    public static PacketIdentity Identity => new("configuration.toClient.ping", "Ping", PacketPhase.Configuration, PacketDirection.Clientbound, 9);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x05;
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
