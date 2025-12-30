using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record HashedSlot(Slot Slot, IReadOnlyList<HashedSlotComponent> Components)
{
    public sealed record HashedSlotComponent(SlotComponentType Type, int Hash);

    public static HashedSlot Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        return reader.ReadHashedSlot(protocolVersion);
    }

    public void Write(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteHashedSlot(this, protocolVersion);
    }
}
