using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Status.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("status.toClient.ping", PacketPhase.Status, PacketDirection.Clientbound)]
[PacketField("Time", "long")]
public sealed partial record PongResponsePacket(long Time) : IPacket<PongResponsePacket>, IPacket
{
    public static PongResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PongResponsePacket>(protocolVersion);
        var time = reader.ReadSignedLong();
        return new PongResponsePacket(time);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PongResponsePacket>(protocolVersion);
        writer.WriteSignedLong(Time);
    }

    public static PacketIdentity Identity => new("status.toClient.ping", "PongResponse", PacketPhase.Status, PacketDirection.Clientbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 776)
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
