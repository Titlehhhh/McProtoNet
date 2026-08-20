using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.game_test_highlight_pos", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("AbsolutePos", "Position")]
[PacketField("RelativePos", "Position")]
public sealed partial record GameTestHighlightPosPacket(Position AbsolutePos, Position RelativePos) : IPacket<GameTestHighlightPosPacket>, IPacket
{
    public static GameTestHighlightPosPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameTestHighlightPosPacket>(protocolVersion);
        var absolutePos = reader.ReadType<Position>(protocolVersion);
        var relativePos = reader.ReadType<Position>(protocolVersion);
        return new GameTestHighlightPosPacket(absolutePos, relativePos);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameTestHighlightPosPacket>(protocolVersion);
        writer.WriteType<Position>(AbsolutePos, protocolVersion);
        writer.WriteType<Position>(RelativePos, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.game_test_highlight_pos", "GameTestHighlightPos", PacketPhase.Play, PacketDirection.Clientbound, 46);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x27;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x28;
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
