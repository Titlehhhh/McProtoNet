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

    public static PacketIdentity Identity => new("play.toClient.low_disk_space_warning", "LowDiskSpaceWarning", PacketPhase.Play, PacketDirection.Clientbound, 53);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x32;
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
