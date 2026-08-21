using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.tab_complete", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TransactionId", "int")]
[PacketField("Start", "int")]
[PacketField("Length", "int")]
[PacketField("Matches", "TabCompleteMatch[]")]
public sealed partial record TabCompletePacket(int TransactionId, int Start, int Length, TabCompleteMatch[] Matches) : IPacket<TabCompletePacket>, IPacket
{
    public static TabCompletePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompletePacket>(protocolVersion);
        var transactionId = reader.ReadVarInt();
        var start = reader.ReadVarInt();
        var length = reader.ReadVarInt();
        int matchesCount = reader.ReadVarInt();
        var matches = new TabCompleteMatch[matchesCount];
        for (int i = 0; i < matches.Length; i++)
            matches[i] = reader.ReadType<TabCompleteMatch>(protocolVersion);
        return new TabCompletePacket(transactionId, start, length, matches);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TabCompletePacket>(protocolVersion);
        writer.WriteVarInt(TransactionId);
        writer.WriteVarInt(Start);
        writer.WriteVarInt(Length);
        writer.WriteVarInt(Matches.Length);
        foreach (var matchesItem in Matches)
            writer.WriteType<TabCompleteMatch>(matchesItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.tab_complete", "TabComplete", PacketPhase.Play, PacketDirection.Clientbound, 100);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x11;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x0F;
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
