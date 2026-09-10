using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.game_test_highlight_pos", "GameTestHighlightPos", PacketPhase.Play, PacketDirection.Clientbound, 49);

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
