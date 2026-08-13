using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.statistics", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Entries", "StatisticEntry[]")]
public sealed partial record StatisticsPacket(StatisticEntry[] Entries) : IPacket<StatisticsPacket>, IPacket
{
    public static StatisticsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StatisticsPacket>(protocolVersion);
        int entriesCount = reader.ReadVarInt();
        var entries = new StatisticEntry[entriesCount];
        for (int i = 0; i < entries.Length; i++)
            entries[i] = reader.ReadType<StatisticEntry>(protocolVersion);
        return new StatisticsPacket(entries);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<StatisticsPacket>(protocolVersion);
        writer.WriteVarInt(Entries.Length);
        foreach (var entriesItem in Entries)
            writer.WriteType<StatisticEntry>(entriesItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.statistics", "Statistics", PacketPhase.Play, PacketDirection.Clientbound, 91);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x05;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x03;
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
