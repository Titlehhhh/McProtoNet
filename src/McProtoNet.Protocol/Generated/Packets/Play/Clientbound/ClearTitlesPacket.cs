using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.clear_titles", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Reset", "bool")]
public sealed partial record ClearTitlesPacket(bool Reset) : IPacket<ClearTitlesPacket>, IPacket
{
    public static ClearTitlesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClearTitlesPacket>(protocolVersion);
        var reset = reader.ReadBoolean();
        return new ClearTitlesPacket(reset);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClearTitlesPacket>(protocolVersion);
        writer.WriteBoolean(Reset);
    }

    public static PacketIdentity Identity => new("play.toClient.clear_titles", "ClearTitles", PacketPhase.Play, PacketDirection.Clientbound, 18);

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
