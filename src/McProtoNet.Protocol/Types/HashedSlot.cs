using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record HashedSlot(Slot Slot, IReadOnlyList<HashedSlotComponent> Components)
{
    public sealed record HashedSlotComponent(SlotComponentType Type, int Hash);

    public static HashedSlot Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HashedSlot>(protocolVersion);
        int itemId = reader.ReadVarInt();
        int itemCount = reader.ReadVarInt();
        HashedSlotComponent[] components = ReadComponents(ref reader, protocolVersion);
        SlotComponentType[] removed = ReadRemovedComponents(ref reader, protocolVersion);
        var slot = new Slot
        {
            ItemId = itemId,
            ItemCount = itemCount,
            RemovedComponents = removed
        };
        return new HashedSlot(slot, components);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HashedSlot>(protocolVersion);
        writer.WriteVarInt(Slot.ItemId ?? 0);
        writer.WriteVarInt(Slot.ItemCount ?? 0);
        writer.WriteVarInt(Components.Count);
        for (int i = 0; i < Components.Count; i++)
        {
            Components[i].Type.Write(ref writer, protocolVersion);
            writer.WriteSignedInt(Components[i].Hash);
        }
        writer.WriteVarInt(Slot.RemovedComponents?.Count ?? 0);
        if (Slot.RemovedComponents is not null)
        {
            foreach (var removed in Slot.RemovedComponents)
            {
                removed.Write(ref writer, protocolVersion);
            }
        }
    }

    private static HashedSlotComponent[] ReadComponents(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<HashedSlotComponent>();
        }

        var components = new HashedSlotComponent[count];
        for (int i = 0; i < count; i++)
        {
            var type = SlotComponentTypeExtensions.Read(ref reader, protocolVersion);
            int hash = reader.ReadSignedInt();
            components[i] = new HashedSlotComponent(type, hash);
        }

        return components;
    }

    private static SlotComponentType[] ReadRemovedComponents(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
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
