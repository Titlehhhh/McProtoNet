using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.tick_end", PacketPhase.Play, PacketDirection.Serverbound)]
public sealed partial record TickEndPacket() : IPacket<TickEndPacket>, IPacket
{
    public static TickEndPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TickEndPacket>(protocolVersion);
        return new TickEndPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TickEndPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.tick_end", "TickEnd", PacketPhase.Play, PacketDirection.Serverbound, 59);

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
