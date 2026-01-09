using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TradeList", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TradeListPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 759),
        new(760, 765),
        new(766, 769),
        new(770, MinecraftVersion.LatestProtocol),
    };

    public int WindowId { get; set; }

    public VFirst_758Fields? VFirst_758 { get; set; }
    public V759Fields? V759 { get; set; }
    public V760_765Fields? V760_765 { get; set; }
    public V766_769Fields? V766_769 { get; set; }
    public V770_LastFields? V770_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                var fields = VFirst_758 ?? throw new InvalidOperationException("TradeList VFirst_758 fields missing.");
                writer.WriteVarInt(WindowId);
                writer.WriteUnsignedByte((byte)fields.Trades.Length);
                for (int i = 0; i < fields.Trades.Length; i++)
                {
                    writer.WriteSlot(fields.Trades[i].InputItem1, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].OutputItem, protocolVersion);
                    if (fields.Trades[i].InputItem2 is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        writer.WriteSlot(fields.Trades[i].InputItem2.Value, protocolVersion);
                    }
                    writer.WriteBoolean(fields.Trades[i].TradeDisabled);
                    writer.WriteSignedInt(fields.Trades[i].NbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].MaximumNbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].Xp);
                    writer.WriteSignedInt(fields.Trades[i].SpecialPrice);
                    writer.WriteFloat(fields.Trades[i].PriceMultiplier);
                    writer.WriteSignedInt(fields.Trades[i].Demand);
                }
                writer.WriteVarInt(fields.VillagerLevel);
                writer.WriteVarInt(fields.Experience);
                writer.WriteBoolean(fields.IsRegularVillager);
                writer.WriteBoolean(fields.CanRestock);
                return;
            }
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("TradeList V759 fields missing.");
                writer.WriteVarInt(WindowId);
                writer.WriteUnsignedByte((byte)fields.Trades.Length);
                for (int i = 0; i < fields.Trades.Length; i++)
                {
                    writer.WriteSlot(fields.Trades[i].InputItem1, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].OutputItem, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].InputItem2, protocolVersion);
                    writer.WriteBoolean(fields.Trades[i].TradeDisabled);
                    writer.WriteSignedInt(fields.Trades[i].NbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].MaximumNbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].Xp);
                    writer.WriteSignedInt(fields.Trades[i].SpecialPrice);
                    writer.WriteFloat(fields.Trades[i].PriceMultiplier);
                    writer.WriteSignedInt(fields.Trades[i].Demand);
                }
                writer.WriteVarInt(fields.VillagerLevel);
                writer.WriteVarInt(fields.Experience);
                writer.WriteBoolean(fields.IsRegularVillager);
                writer.WriteBoolean(fields.CanRestock);
                return;
            }
            case >= 760 and <= 765:
            {
                var fields = V760_765 ?? throw new InvalidOperationException("TradeList V760_765 fields missing.");
                writer.WriteVarInt(WindowId);
                writer.WriteVarInt(fields.Trades.Length);
                for (int i = 0; i < fields.Trades.Length; i++)
                {
                    writer.WriteSlot(fields.Trades[i].InputItem1, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].OutputItem, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].InputItem2, protocolVersion);
                    writer.WriteBoolean(fields.Trades[i].TradeDisabled);
                    writer.WriteSignedInt(fields.Trades[i].NbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].MaximumNbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].Xp);
                    writer.WriteSignedInt(fields.Trades[i].SpecialPrice);
                    writer.WriteFloat(fields.Trades[i].PriceMultiplier);
                    writer.WriteSignedInt(fields.Trades[i].Demand);
                }
                writer.WriteVarInt(fields.VillagerLevel);
                writer.WriteVarInt(fields.Experience);
                writer.WriteBoolean(fields.IsRegularVillager);
                writer.WriteBoolean(fields.CanRestock);
                return;
            }
            case >= 766 and <= 769:
            {
                var fields = V766_769 ?? throw new InvalidOperationException("TradeList V766_769 fields missing.");
                WriteContainerId(ref writer, WindowId, protocolVersion);
                writer.WriteVarInt(fields.Trades.Length);
                for (int i = 0; i < fields.Trades.Length; i++)
                {
                    WriteItemWithComponents(ref writer, fields.Trades[i].InputItem1, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].OutputItem, protocolVersion);
                    if (fields.Trades[i].InputItem2 is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        WriteItemWithComponents(ref writer, fields.Trades[i].InputItem2.Value, protocolVersion);
                    }
                    writer.WriteBoolean(fields.Trades[i].TradeDisabled);
                    writer.WriteSignedInt(fields.Trades[i].NbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].MaximumNbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].Xp);
                    writer.WriteSignedInt(fields.Trades[i].SpecialPrice);
                    writer.WriteFloat(fields.Trades[i].PriceMultiplier);
                    writer.WriteSignedInt(fields.Trades[i].Demand);
                }
                writer.WriteVarInt(fields.VillagerLevel);
                writer.WriteVarInt(fields.Experience);
                writer.WriteBoolean(fields.IsRegularVillager);
                writer.WriteBoolean(fields.CanRestock);
                return;
            }
            case >= 770 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V770_Last ?? throw new InvalidOperationException("TradeList V770_Last fields missing.");
                WriteContainerId(ref writer, WindowId, protocolVersion);
                writer.WriteVarInt(fields.Trades.Length);
                for (int i = 0; i < fields.Trades.Length; i++)
                {
                    WriteExactItem(ref writer, fields.Trades[i].InputItem1, protocolVersion);
                    writer.WriteSlot(fields.Trades[i].OutputItem, protocolVersion);
                    if (fields.Trades[i].InputItem2 is null)
                    {
                        writer.WriteBoolean(false);
                    }
                    else
                    {
                        writer.WriteBoolean(true);
                        WriteExactItem(ref writer, fields.Trades[i].InputItem2.Value, protocolVersion);
                    }
                    writer.WriteBoolean(fields.Trades[i].TradeDisabled);
                    writer.WriteSignedInt(fields.Trades[i].NbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].MaximumNbTradeUses);
                    writer.WriteSignedInt(fields.Trades[i].Xp);
                    writer.WriteSignedInt(fields.Trades[i].SpecialPrice);
                    writer.WriteFloat(fields.Trades[i].PriceMultiplier);
                    writer.WriteSignedInt(fields.Trades[i].Demand);
                }
                writer.WriteVarInt(fields.VillagerLevel);
                writer.WriteVarInt(fields.Experience);
                writer.WriteBoolean(fields.IsRegularVillager);
                writer.WriteBoolean(fields.CanRestock);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TradeList), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                WindowId = reader.ReadVarInt();
                int count = reader.ReadUnsignedByte();
                var trades = new TradeEntryVFirst_758[count];
                for (int i = 0; i < trades.Length; i++)
                {
                    Slot input1 = reader.ReadSlot(protocolVersion);
                    Slot output = reader.ReadSlot(protocolVersion);
                    Slot? input2 = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadSlot(protocolVersion));
                    trades[i] = new TradeEntryVFirst_758
                    {
                        InputItem1 = input1,
                        OutputItem = output,
                        InputItem2 = input2,
                        TradeDisabled = reader.ReadBoolean(),
                        NbTradeUses = reader.ReadSignedInt(),
                        MaximumNbTradeUses = reader.ReadSignedInt(),
                        Xp = reader.ReadSignedInt(),
                        SpecialPrice = reader.ReadSignedInt(),
                        PriceMultiplier = reader.ReadFloat(),
                        Demand = reader.ReadSignedInt()
                    };
                }
                VFirst_758 = new VFirst_758Fields
                {
                    Trades = trades,
                    VillagerLevel = reader.ReadVarInt(),
                    Experience = reader.ReadVarInt(),
                    IsRegularVillager = reader.ReadBoolean(),
                    CanRestock = reader.ReadBoolean()
                };
                return;
            }
            case 759:
            {
                WindowId = reader.ReadVarInt();
                int count = reader.ReadUnsignedByte();
                var trades = new TradeEntryV759[count];
                for (int i = 0; i < trades.Length; i++)
                {
                    trades[i] = new TradeEntryV759
                    {
                        InputItem1 = reader.ReadSlot(protocolVersion),
                        OutputItem = reader.ReadSlot(protocolVersion),
                        InputItem2 = reader.ReadSlot(protocolVersion),
                        TradeDisabled = reader.ReadBoolean(),
                        NbTradeUses = reader.ReadSignedInt(),
                        MaximumNbTradeUses = reader.ReadSignedInt(),
                        Xp = reader.ReadSignedInt(),
                        SpecialPrice = reader.ReadSignedInt(),
                        PriceMultiplier = reader.ReadFloat(),
                        Demand = reader.ReadSignedInt()
                    };
                }
                V759 = new V759Fields
                {
                    Trades = trades,
                    VillagerLevel = reader.ReadVarInt(),
                    Experience = reader.ReadVarInt(),
                    IsRegularVillager = reader.ReadBoolean(),
                    CanRestock = reader.ReadBoolean()
                };
                return;
            }
            case >= 760 and <= 765:
            {
                WindowId = reader.ReadVarInt();
                int count = reader.ReadVarInt();
                var trades = new TradeEntryV760_765[count];
                for (int i = 0; i < trades.Length; i++)
                {
                    trades[i] = new TradeEntryV760_765
                    {
                        InputItem1 = reader.ReadSlot(protocolVersion),
                        OutputItem = reader.ReadSlot(protocolVersion),
                        InputItem2 = reader.ReadSlot(protocolVersion),
                        TradeDisabled = reader.ReadBoolean(),
                        NbTradeUses = reader.ReadSignedInt(),
                        MaximumNbTradeUses = reader.ReadSignedInt(),
                        Xp = reader.ReadSignedInt(),
                        SpecialPrice = reader.ReadSignedInt(),
                        PriceMultiplier = reader.ReadFloat(),
                        Demand = reader.ReadSignedInt()
                    };
                }
                V760_765 = new V760_765Fields
                {
                    Trades = trades,
                    VillagerLevel = reader.ReadVarInt(),
                    Experience = reader.ReadVarInt(),
                    IsRegularVillager = reader.ReadBoolean(),
                    CanRestock = reader.ReadBoolean()
                };
                return;
            }
            case >= 766 and <= 769:
            {
                WindowId = ReadContainerId(ref reader, protocolVersion);
                int count = reader.ReadVarInt();
                var trades = new TradeEntryV766_769[count];
                for (int i = 0; i < trades.Length; i++)
                {
                    trades[i] = new TradeEntryV766_769
                    {
                        InputItem1 = ReadItemWithComponents(ref reader, protocolVersion),
                        OutputItem = reader.ReadSlot(protocolVersion),
                        InputItem2 = reader.ReadOptional((ref MinecraftPrimitiveReader r) => ReadItemWithComponents(ref r, protocolVersion)),
                        TradeDisabled = reader.ReadBoolean(),
                        NbTradeUses = reader.ReadSignedInt(),
                        MaximumNbTradeUses = reader.ReadSignedInt(),
                        Xp = reader.ReadSignedInt(),
                        SpecialPrice = reader.ReadSignedInt(),
                        PriceMultiplier = reader.ReadFloat(),
                        Demand = reader.ReadSignedInt()
                    };
                }
                V766_769 = new V766_769Fields
                {
                    Trades = trades,
                    VillagerLevel = reader.ReadVarInt(),
                    Experience = reader.ReadVarInt(),
                    IsRegularVillager = reader.ReadBoolean(),
                    CanRestock = reader.ReadBoolean()
                };
                return;
            }
            case >= 770 and <= MinecraftVersion.LatestProtocol:
            {
                WindowId = ReadContainerId(ref reader, protocolVersion);
                int count = reader.ReadVarInt();
                var trades = new TradeEntryV770_Last[count];
                for (int i = 0; i < trades.Length; i++)
                {
                    trades[i] = new TradeEntryV770_Last
                    {
                        InputItem1 = ReadExactItem(ref reader, protocolVersion),
                        OutputItem = reader.ReadSlot(protocolVersion),
                        InputItem2 = reader.ReadOptional((ref MinecraftPrimitiveReader r) => ReadExactItem(ref r, protocolVersion)),
                        TradeDisabled = reader.ReadBoolean(),
                        NbTradeUses = reader.ReadSignedInt(),
                        MaximumNbTradeUses = reader.ReadSignedInt(),
                        Xp = reader.ReadSignedInt(),
                        SpecialPrice = reader.ReadSignedInt(),
                        PriceMultiplier = reader.ReadFloat(),
                        Demand = reader.ReadSignedInt()
                    };
                }
                V770_Last = new V770_LastFields
                {
                    Trades = trades,
                    VillagerLevel = reader.ReadVarInt(),
                    Experience = reader.ReadVarInt(),
                    IsRegularVillager = reader.ReadBoolean(),
                    CanRestock = reader.ReadBoolean()
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.TradeList), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static int ReadContainerId(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return protocolVersion <= 767 ? reader.ReadUnsignedByte() : reader.ReadVarInt();
    }

    private static void WriteContainerId(ref MinecraftPrimitiveWriter writer, int value, int protocolVersion)
    {
        if (protocolVersion <= 767)
        {
            writer.WriteUnsignedByte((byte)value);
        }
        else
        {
            writer.WriteVarInt(value);
        }
    }

    private static ItemWithComponents ReadItemWithComponents(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int itemId = reader.ReadVarInt();
        int itemCount = reader.ReadVarInt();
        int componentCount = reader.ReadVarInt();
        var components = new SlotComponent[componentCount];
        for (int i = 0; i < components.Length; i++)
        {
            components[i] = reader.ReadSlotComponent(protocolVersion);
        }
        return new ItemWithComponents
        {
            ItemId = itemId,
            ItemCount = itemCount,
            Components = components
        };
    }

    private static void WriteItemWithComponents(ref MinecraftPrimitiveWriter writer, ItemWithComponents item, int protocolVersion)
    {
        writer.WriteVarInt(item.ItemId);
        writer.WriteVarInt(item.ItemCount);
        writer.WriteVarInt(item.Components.Length);
        for (int i = 0; i < item.Components.Length; i++)
        {
            writer.WriteSlotComponent(item.Components[i], protocolVersion);
        }
    }

    private static ExactItem ReadExactItem(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return new ExactItem
        {
            ItemId = reader.ReadVarInt(),
            ItemCount = reader.ReadVarInt(),
            Components = reader.ReadExactComponentMatcher(protocolVersion)
        };
    }

    private static void WriteExactItem(ref MinecraftPrimitiveWriter writer, ExactItem item, int protocolVersion)
    {
        writer.WriteVarInt(item.ItemId);
        writer.WriteVarInt(item.ItemCount);
        writer.WriteExactComponentMatcher(item.Components, protocolVersion);
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_758Fields
    {
        public TradeEntryVFirst_758[] Trades { get; set; }
        public int VillagerLevel { get; set; }
        public int Experience { get; set; }
        public bool IsRegularVillager { get; set; }
        public bool CanRestock { get; set; }
    }

    public struct V759Fields
    {
        public TradeEntryV759[] Trades { get; set; }
        public int VillagerLevel { get; set; }
        public int Experience { get; set; }
        public bool IsRegularVillager { get; set; }
        public bool CanRestock { get; set; }
    }

    public struct V760_765Fields
    {
        public TradeEntryV760_765[] Trades { get; set; }
        public int VillagerLevel { get; set; }
        public int Experience { get; set; }
        public bool IsRegularVillager { get; set; }
        public bool CanRestock { get; set; }
    }

    public struct V766_769Fields
    {
        public TradeEntryV766_769[] Trades { get; set; }
        public int VillagerLevel { get; set; }
        public int Experience { get; set; }
        public bool IsRegularVillager { get; set; }
        public bool CanRestock { get; set; }
    }

    public struct V770_LastFields
    {
        public TradeEntryV770_Last[] Trades { get; set; }
        public int VillagerLevel { get; set; }
        public int Experience { get; set; }
        public bool IsRegularVillager { get; set; }
        public bool CanRestock { get; set; }
    }

    public struct TradeEntryVFirst_758
    {
        public Slot InputItem1 { get; set; }
        public Slot OutputItem { get; set; }
        public Slot? InputItem2 { get; set; }
        public bool TradeDisabled { get; set; }
        public int NbTradeUses { get; set; }
        public int MaximumNbTradeUses { get; set; }
        public int Xp { get; set; }
        public int SpecialPrice { get; set; }
        public float PriceMultiplier { get; set; }
        public int Demand { get; set; }
    }

    public struct TradeEntryV759
    {
        public Slot InputItem1 { get; set; }
        public Slot OutputItem { get; set; }
        public Slot InputItem2 { get; set; }
        public bool TradeDisabled { get; set; }
        public int NbTradeUses { get; set; }
        public int MaximumNbTradeUses { get; set; }
        public int Xp { get; set; }
        public int SpecialPrice { get; set; }
        public float PriceMultiplier { get; set; }
        public int Demand { get; set; }
    }

    public struct TradeEntryV760_765
    {
        public Slot InputItem1 { get; set; }
        public Slot OutputItem { get; set; }
        public Slot InputItem2 { get; set; }
        public bool TradeDisabled { get; set; }
        public int NbTradeUses { get; set; }
        public int MaximumNbTradeUses { get; set; }
        public int Xp { get; set; }
        public int SpecialPrice { get; set; }
        public float PriceMultiplier { get; set; }
        public int Demand { get; set; }
    }

    public struct TradeEntryV766_769
    {
        public ItemWithComponents InputItem1 { get; set; }
        public Slot OutputItem { get; set; }
        public ItemWithComponents? InputItem2 { get; set; }
        public bool TradeDisabled { get; set; }
        public int NbTradeUses { get; set; }
        public int MaximumNbTradeUses { get; set; }
        public int Xp { get; set; }
        public int SpecialPrice { get; set; }
        public float PriceMultiplier { get; set; }
        public int Demand { get; set; }
    }

    public struct TradeEntryV770_Last
    {
        public ExactItem InputItem1 { get; set; }
        public Slot OutputItem { get; set; }
        public ExactItem? InputItem2 { get; set; }
        public bool TradeDisabled { get; set; }
        public int NbTradeUses { get; set; }
        public int MaximumNbTradeUses { get; set; }
        public int Xp { get; set; }
        public int SpecialPrice { get; set; }
        public float PriceMultiplier { get; set; }
        public int Demand { get; set; }
    }

    public struct ItemWithComponents
    {
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public SlotComponent[] Components { get; set; }
    }

    public struct ExactItem
    {
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public ExactComponentMatcher Components { get; set; }
    }
}
