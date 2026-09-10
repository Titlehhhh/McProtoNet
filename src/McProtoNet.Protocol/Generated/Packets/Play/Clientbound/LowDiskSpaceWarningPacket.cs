using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.low_disk_space_warning", PacketPhase.Play, PacketDirection.Clientbound)]
public sealed partial record LowDiskSpaceWarningPacket() : IPacket<LowDiskSpaceWarningPacket>, IPacket
{
    public static LowDiskSpaceWarningPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LowDiskSpaceWarningPacket>(protocolVersion);
        return new LowDiskSpaceWarningPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LowDiskSpaceWarningPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.low_disk_space_warning", "LowDiskSpaceWarning", PacketPhase.Play, PacketDirection.Clientbound, 57);

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
