using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Statistics", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class StatisticsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)
    };

    public StatisticEntry[] Entries { get; set; } = Array.Empty<StatisticEntry>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Entries.Length);
                for (int i = 0; i < Entries.Length; i++)
                {
                    writer.WriteVarInt(Entries[i].CategoryId);
                    writer.WriteVarInt(Entries[i].StatisticId);
                    writer.WriteVarInt(Entries[i].Value);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Statistics), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                int length = reader.ReadVarInt();
                if (length == 0)
                {
                    Entries = Array.Empty<StatisticEntry>();
                    return;
                }

                var entries = new StatisticEntry[length];
                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i] = new StatisticEntry
                    {
                        CategoryId = reader.ReadVarInt(),
                        StatisticId = reader.ReadVarInt(),
                        Value = reader.ReadVarInt()
                    };
                }
                Entries = entries;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Statistics), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct StatisticEntry
    {
        public int CategoryId { get; set; }
        public int StatisticId { get; set; }
        public int Value { get; set; }
    }
}
