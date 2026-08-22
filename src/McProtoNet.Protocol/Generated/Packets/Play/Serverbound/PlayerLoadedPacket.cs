using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.player_loaded", PacketPhase.Play, PacketDirection.Serverbound)]
public sealed partial record PlayerLoadedPacket() : IPacket<PlayerLoadedPacket>, IPacket
{
    public static PlayerLoadedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerLoadedPacket>(protocolVersion);
        return new PlayerLoadedPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerLoadedPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.player_loaded", "PlayerLoaded", PacketPhase.Play, PacketDirection.Serverbound, 36);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x2A;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x2B;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x2C;
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
