using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Status.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("status.toServer.ping", PacketPhase.Status, PacketDirection.Serverbound)]
[PacketField("Time", "long")]
public sealed partial record PingRequestPacket(long Time) : IPacket<PingRequestPacket>, IPacket
{
    public static PingRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingRequestPacket>(protocolVersion);
        var time = reader.ReadSignedLong();
        return new PingRequestPacket(time);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingRequestPacket>(protocolVersion);
        writer.WriteSignedLong(Time);
    }

    public static PacketIdentity Identity => new("status.toServer.ping", "PingRequest", PacketPhase.Status, PacketDirection.Serverbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
        {
            id = 0x01;
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
