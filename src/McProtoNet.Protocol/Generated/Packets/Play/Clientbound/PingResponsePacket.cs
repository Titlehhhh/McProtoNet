using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.ping_response", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Id", "long")]
public sealed partial record PingResponsePacket(long Id) : IPacket<PingResponsePacket>, IPacket
{
    public static PingResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingResponsePacket>(protocolVersion);
        var id = reader.ReadSignedLong();
        return new PingResponsePacket(id);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingResponsePacket>(protocolVersion);
        writer.WriteSignedLong(Id);
    }

    public static PacketIdentity Identity => new("play.toClient.ping_response", "PingResponse", PacketPhase.Play, PacketDirection.Clientbound, 69);

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
