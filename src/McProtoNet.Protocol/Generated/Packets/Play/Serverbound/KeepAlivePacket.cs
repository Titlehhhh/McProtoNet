using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.keep_alive", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("KeepAliveId", "long")]
public sealed partial record KeepAlivePacket(long KeepAliveId) : IPacket<KeepAlivePacket>, IPacket
{
    public static KeepAlivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KeepAlivePacket>(protocolVersion);
        var keepAliveId = reader.ReadSignedLong();
        return new KeepAlivePacket(keepAliveId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KeepAlivePacket>(protocolVersion);
        writer.WriteSignedLong(KeepAliveId);
    }

    public static PacketIdentity Identity => new("play.toServer.keep_alive", "KeepAlive", PacketPhase.Play, PacketDirection.Serverbound, 29);

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
