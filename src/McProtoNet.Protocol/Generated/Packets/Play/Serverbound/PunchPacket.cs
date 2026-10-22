using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(777, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.punch", PacketPhase.Play, PacketDirection.Serverbound)]
public sealed partial record PunchPacket() : IPacket<PunchPacket>, IPacket
{
    public static PunchPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PunchPacket>(protocolVersion);
        return new PunchPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PunchPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.punch", "Punch", PacketPhase.Play, PacketDirection.Serverbound, 44);

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
