using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Statistics", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x06)]
[PacketId(751, 754, 0x06)]
[PacketId(755, 758, 0x07)]
[PacketId(759, 761, 0x04)]
[PacketId(762, 763, 0x05)]
[PacketId(764, 769, 0x04)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class StatisticsPacket : IServerPacket
{
    public StatisticEntry[] Entries { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Entries.Length);
        foreach (var entry in Entries)
        {
            writer.WriteVarInt(entry.CategoryId);
            writer.WriteVarInt(entry.StatisticId);
            writer.WriteVarInt(entry.Value);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        var entries = new StatisticEntry[count];
        for (int i = 0; i < count; i++)
        {
            entries[i] = new StatisticEntry
            {
                CategoryId = reader.ReadVarInt(),
                StatisticId = reader.ReadVarInt(),
                Value = reader.ReadVarInt()
            };
        }
        Entries = entries;
    }

    public struct StatisticEntry
    {
        public int CategoryId { get; set; }
        public int StatisticId { get; set; }
        public int Value { get; set; }
    }
}