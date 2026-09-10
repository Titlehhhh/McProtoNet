using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.statistics", "Statistics", PacketPhase.Play, PacketDirection.Clientbound, 104);

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
