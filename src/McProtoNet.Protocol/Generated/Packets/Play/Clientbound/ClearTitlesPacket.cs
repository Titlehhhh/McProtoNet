using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toClient.clear_titles", "ClearTitles", PacketPhase.Play, PacketDirection.Clientbound, 17);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x0E;
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
