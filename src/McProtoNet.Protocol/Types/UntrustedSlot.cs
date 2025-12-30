using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record UntrustedSlot(Slot Slot, IReadOnlyList<UntrustedSlotComponent> Components)
{
    public static UntrustedSlot Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlot>(protocolVersion);
        int itemCount = reader.ReadVarInt();
        if (itemCount == 0)
        {
            return new UntrustedSlot(new Slot { ItemCount = 0 }, Array.Empty<UntrustedSlotComponent>());
        }

        int itemId = reader.ReadVarInt();
        int addedCount = reader.ReadVarInt();
        int removedCount = reader.ReadVarInt();
        UntrustedSlotComponent[] components = ReadComponents(ref reader, protocolVersion, addedCount);
        SlotComponentType[] removed = ReadRemovedComponents(ref reader, protocolVersion, removedCount);

        var slot = new Slot
        {
            ItemId = itemId,
            ItemCount = itemCount,
            RemovedComponents = removed
        };

        return new UntrustedSlot(slot, components);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlot>(protocolVersion);
        int count = Slot.ItemCount.GetValueOrDefault(0);
        writer.WriteVarInt(count);
        if (count == 0)
        {
            return;
        }

        writer.WriteVarInt(Slot.ItemId ?? 0);
        writer.WriteVarInt(Components.Count);
        writer.WriteVarInt(Slot.RemovedComponents?.Count ?? 0);
        for (int i = 0; i < Components.Count; i++)
        {
            Components[i].Write(ref writer, protocolVersion);
        }
        if (Slot.RemovedComponents is not null)
        {
            foreach (var removed in Slot.RemovedComponents)
            {
                removed.Write(ref writer, protocolVersion);
            }
        }
    }

    private static UntrustedSlotComponent[] ReadComponents(ref MinecraftPrimitiveReader reader, int protocolVersion, int count)
    {
        if (count == 0)
        {
            return Array.Empty<UntrustedSlotComponent>();
        }

        var components = new UntrustedSlotComponent[count];
        for (int i = 0; i < count; i++)
        {
            components[i] = UntrustedSlotComponent.Read(ref reader, protocolVersion);
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
}
