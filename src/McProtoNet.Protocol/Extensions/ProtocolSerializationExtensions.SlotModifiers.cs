using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static UntrustedSlotComponent ReadUntrustedSlotComponent(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlotComponent>(protocolVersion);
        var type = reader.ReadSlotComponentType(protocolVersion);
        int length = reader.ReadVarInt();
        var data = reader.ReadBuffer(length);
        return new UntrustedSlotComponent(type, data);
    }

    public static void WriteUntrustedSlotComponent(this ref MinecraftPrimitiveWriter writer, UntrustedSlotComponent component,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlotComponent>(protocolVersion);
        writer.WriteSlotComponentType(component.Type, protocolVersion);
        writer.WriteVarInt(component.Data.Length);
        writer.WriteBuffer(component.Data);
    }

    public static UntrustedSlot ReadUntrustedSlot(this ref MinecraftPrimitiveReader reader, int protocolVersion)
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
        UntrustedSlotComponent[] components = ReadUntrustedComponents(ref reader, protocolVersion, addedCount);
        SlotComponentType[] removed = ReadUntrustedRemovedComponents(ref reader, protocolVersion, removedCount);

        var slot = new Slot
        {
            ItemId = itemId,
            ItemCount = itemCount,
            RemovedComponents = removed
        };

        return new UntrustedSlot(slot, components);
    }

    public static void WriteUntrustedSlot(this ref MinecraftPrimitiveWriter writer, UntrustedSlot untrustedSlot, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UntrustedSlot>(protocolVersion);
        int count = untrustedSlot.Slot.ItemCount.GetValueOrDefault(0);
        writer.WriteVarInt(count);
        if (count == 0)
        {
            return;
        }

        writer.WriteVarInt(untrustedSlot.Slot.ItemId ?? 0);
        writer.WriteVarInt(untrustedSlot.Components.Count);
        writer.WriteVarInt(untrustedSlot.Slot.RemovedComponents?.Count ?? 0);
        for (int i = 0; i < untrustedSlot.Components.Count; i++)
        {
            writer.WriteUntrustedSlotComponent(untrustedSlot.Components[i], protocolVersion);
        }
        if (untrustedSlot.Slot.RemovedComponents is not null)
        {
            foreach (var removed in untrustedSlot.Slot.RemovedComponents)
            {
                writer.WriteSlotComponentType(removed, protocolVersion);
            }
        }
    }

    private static UntrustedSlotComponent[] ReadUntrustedComponents(ref MinecraftPrimitiveReader reader, int protocolVersion, int count)
    {
        if (count == 0)
        {
            return Array.Empty<UntrustedSlotComponent>();
        }

        var components = new UntrustedSlotComponent[count];
        for (int i = 0; i < count; i++)
        {
            components[i] = reader.ReadUntrustedSlotComponent(protocolVersion);
        }

        return components;
    }

    private static SlotComponentType[] ReadUntrustedRemovedComponents(ref MinecraftPrimitiveReader reader, int protocolVersion,
        int count)
    {
        if (count == 0)
        {
            return Array.Empty<SlotComponentType>();
        }

        var components = new SlotComponentType[count];
        for (int i = 0; i < count; i++)
        {
            components[i] = reader.ReadSlotComponentType(protocolVersion);
        }

        return components;
    }

    public static HashedSlot ReadHashedSlot(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HashedSlot>(protocolVersion);
        int itemId = reader.ReadVarInt();
        int itemCount = reader.ReadVarInt();
        HashedSlotComponent[] components = ReadHashedComponents(ref reader, protocolVersion);
        SlotComponentType[] removed = ReadHashedRemovedComponents(ref reader, protocolVersion);
        var slot = new Slot
        {
            ItemId = itemId,
            ItemCount = itemCount,
            RemovedComponents = removed
        };
        return new HashedSlot(slot, components);
    }

    public static void WriteHashedSlot(this ref MinecraftPrimitiveWriter writer, HashedSlot hashedSlot, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HashedSlot>(protocolVersion);
        writer.WriteVarInt(hashedSlot.Slot.ItemId ?? 0);
        writer.WriteVarInt(hashedSlot.Slot.ItemCount ?? 0);
        writer.WriteVarInt(hashedSlot.Components.Count);
        for (int i = 0; i < hashedSlot.Components.Count; i++)
        {
            writer.WriteSlotComponentType(hashedSlot.Components[i].Type, protocolVersion);
            writer.WriteSignedInt(hashedSlot.Components[i].Hash);
        }
        writer.WriteVarInt(hashedSlot.Slot.RemovedComponents?.Count ?? 0);
        if (hashedSlot.Slot.RemovedComponents is not null)
        {
            foreach (var removed in hashedSlot.Slot.RemovedComponents)
            {
                writer.WriteSlotComponentType(removed, protocolVersion);
            }
        }
    }

    private static HashedSlotComponent[] ReadHashedComponents(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<HashedSlotComponent>();
        }

        var components = new HashedSlotComponent[count];
        for (int i = 0; i < count; i++)
        {
            var type = reader.ReadSlotComponentType(protocolVersion);
            int hash = reader.ReadSignedInt();
            components[i] = new HashedSlotComponent(type, hash);
        }

        return components;
    }

    private static SlotComponentType[] ReadHashedRemovedComponents(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<SlotComponentType>();
        }

        var components = new SlotComponentType[count];
        for (int i = 0; i < count; i++)
        {
            components[i] = reader.ReadSlotComponentType(protocolVersion);
        }

        return components;
    }
}
