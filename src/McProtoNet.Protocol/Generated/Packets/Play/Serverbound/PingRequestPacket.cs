using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.ping_request", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Id", "long")]
public sealed partial record PingRequestPacket(long Id) : IPacket<PingRequestPacket>, IPacket
{
    public static PingRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingRequestPacket>(protocolVersion);
        var id = reader.ReadSignedLong();
        return new PingRequestPacket(id);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingRequestPacket>(protocolVersion);
        writer.WriteSignedLong(Id);
    }

    public static PacketIdentity Identity => new("play.toServer.ping_request", "PingRequest", PacketPhase.Play, PacketDirection.Serverbound, 36);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
