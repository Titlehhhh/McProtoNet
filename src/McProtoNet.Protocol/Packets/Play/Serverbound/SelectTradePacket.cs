using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

[PacketInfo("SelectTrade", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x22)]
[PacketId(751, 758, 0x23)]
[PacketId(759, 759, 0x25)]
[PacketId(760, 763, 0x26)]
[PacketId(764, 764, 0x29)]
[PacketId(765, 765, 0x2A)]
[PacketId(766, 767, 0x2D)]
[PacketId(768, 768, 0x2F)]
[PacketId(769, 770, 0x31)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x32)]
public sealed partial class SelectTradePacket : IClientPacket
{
    public List<TradeEntry> Trades { get; set; } = new();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteVarInt(Trades.Count);
                foreach (var entry in Trades)
                {
                    writer.WriteType(entry.Slot, protocolVersion);
                    writer.WriteVarInt(entry.SomeInt);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SelectTradePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
            {
                var count = reader.ReadVarInt();
                var list = new List<TradeEntry>(count);
                for (int i = 0; i < count; i++)
                {
                    var slot = reader.ReadType<Slot>(protocolVersion);
                    var someInt = reader.ReadVarInt();
                    list.Add(new TradeEntry { Slot = slot, SomeInt = someInt });
                }
                Trades = list;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SelectTradePacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct TradeEntry
    {
        public Slot Slot { get; set; }
        public int SomeInt { get; set; }
    }
}