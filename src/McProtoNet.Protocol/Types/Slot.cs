using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Slot
{
    public int? ItemId { get; set; }
    public int? ItemCount { get; set; }
    public NbtTag? Nbt { get; set; }
    public IReadOnlyList<SlotComponent>? Components { get; set; }
    public IReadOnlyList<SlotComponentType>? RemovedComponents { get; set; }

    public static Slot Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Slot>(protocolVersion);
        if (protocolVersion <= 763)
        {
            bool present = reader.ReadBoolean();
            if (!present)
            {
                return new Slot();
            }

            int itemId = reader.ReadVarInt();
            sbyte itemCount = reader.ReadSignedByte();
            NbtTag? nbt = reader.ReadOptionalNbtTag(protocolVersion);
            return new Slot
            {
                ItemId = itemId,
                ItemCount = itemCount,
                Nbt = nbt
            };
        }

        if (protocolVersion <= 765)
        {
            bool present = reader.ReadBoolean();
            if (!present)
            {
                return new Slot();
            }

            int itemId = reader.ReadVarInt();
            sbyte itemCount = reader.ReadSignedByte();
            NbtTag? nbt = reader.ReadAnonOptionalNbtTag(protocolVersion);
            return new Slot
            {
                ItemId = itemId,
                ItemCount = itemCount,
                Nbt = nbt
            };
        }

        if (protocolVersion == 766)
        {
            sbyte itemCount = reader.ReadSignedByte();
            if (itemCount == 0)
            {
                return new Slot { ItemCount = 0 };
            }

            int itemId = reader.ReadVarInt();
            int addedCount = reader.ReadVarInt();
            int removedCount = reader.ReadVarInt();
            SlotComponent[] components = ReadComponents(ref reader, protocolVersion, addedCount);
            SlotComponentType[] removed = ReadRemovedComponents(ref reader, protocolVersion, removedCount);
            return new Slot
            {
                ItemId = itemId,
                ItemCount = itemCount,
                Components = components,
                RemovedComponents = removed
            };
        }

        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return new Slot { ItemCount = 0 };
        }

        int itemIdNew = reader.ReadVarInt();
        int addedComponentCount = reader.ReadVarInt();
        int removedComponentCount = reader.ReadVarInt();
        SlotComponent[] addedComponents = ReadComponents(ref reader, protocolVersion, addedComponentCount);
        SlotComponentType[] removedComponents = ReadRemovedComponents(ref reader, protocolVersion, removedComponentCount);
        return new Slot
        {
            ItemId = itemIdNew,
            ItemCount = count,
            Components = addedComponents,
            RemovedComponents = removedComponents
        };
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Slot>(protocolVersion);
        if (protocolVersion <= 763)
        {
            if (ItemId is null || ItemCount is null)
            {
                writer.WriteBoolean(false);
                return;
            }

            writer.WriteBoolean(true);
            writer.WriteVarInt(ItemId.Value);
            writer.WriteSignedByte((sbyte)ItemCount.Value);
            writer.WriteOptionalNbtTag(Nbt, protocolVersion);
            return;
        }

        if (protocolVersion <= 765)
        {
            if (ItemId is null || ItemCount is null)
            {
                writer.WriteBoolean(false);
                return;
            }

            writer.WriteBoolean(true);
            writer.WriteVarInt(ItemId.Value);
            writer.WriteSignedByte((sbyte)ItemCount.Value);
            writer.WriteAnonOptionalNbtTag(Nbt, protocolVersion);
            return;
        }

        int count = ItemCount.GetValueOrDefault(0);
        if (protocolVersion == 766)
        {
            writer.WriteSignedByte((sbyte)count);
            if (count == 0)
            {
                return;
            }

            writer.WriteVarInt(ItemId ?? 0);
            WriteComponentCounts(ref writer, Components, RemovedComponents);
            WriteComponentsWithoutCount(ref writer, protocolVersion, Components ?? Array.Empty<SlotComponent>());
            WriteRemovedComponentsWithoutCount(ref writer, protocolVersion, RemovedComponents ?? Array.Empty<SlotComponentType>());
            return;
        }

        writer.WriteVarInt(count);
        if (count == 0)
        {
            return;
        }

        writer.WriteVarInt(ItemId ?? 0);
        WriteComponentCounts(ref writer, Components, RemovedComponents);
        WriteComponentsWithoutCount(ref writer, protocolVersion, Components ?? Array.Empty<SlotComponent>());
        WriteRemovedComponentsWithoutCount(ref writer, protocolVersion, RemovedComponents ?? Array.Empty<SlotComponentType>());
    }

    private static SlotComponent[] ReadComponents(ref MinecraftPrimitiveReader reader, int protocolVersion, int count)
    {
        if (count == 0)
        {
            return Array.Empty<SlotComponent>();
        }

        var components = new SlotComponent[count];
        for (int i = 0; i < count; i++)
        {
            components[i] = SlotComponent.Read(ref reader, protocolVersion);
        }

        return components;
    }

    private static SlotComponentType[] ReadRemovedComponents(ref MinecraftPrimitiveReader reader, int protocolVersion, int count)
    {
        if (count == 0)
        {
            return Array.Empty<SlotComponentType>();
        }

        var components = new SlotComponentType[count];
        for (int i = 0; i < count; i++)
        {
            components[i] = SlotComponentTypeExtensions.Read(ref reader, protocolVersion);
        }

        return components;
    }

    private static void WriteComponentCounts(ref MinecraftPrimitiveWriter writer, IReadOnlyList<SlotComponent>? components,
        IReadOnlyList<SlotComponentType>? removed)
    {
        writer.WriteVarInt(components?.Count ?? 0);
        writer.WriteVarInt(removed?.Count ?? 0);
    }

    private static void WriteComponentsWithoutCount(ref MinecraftPrimitiveWriter writer, int protocolVersion,
        IReadOnlyList<SlotComponent> components)
    {
        for (int i = 0; i < components.Count; i++)
        {
            components[i].Write(ref writer, protocolVersion);
        }
    }

    private static void WriteRemovedComponentsWithoutCount(ref MinecraftPrimitiveWriter writer, int protocolVersion,
        IReadOnlyList<SlotComponentType> removed)
    {
        for (int i = 0; i < removed.Count; i++)
        {
            removed[i].Write(ref writer, protocolVersion);
        }
    }
}
