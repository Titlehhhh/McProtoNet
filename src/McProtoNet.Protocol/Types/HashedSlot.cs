using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record HashedSlot(Slot Slot, IReadOnlyList<HashedSlotComponent> Components);

public sealed partial record HashedSlotComponent(SlotComponentType Type, int Hash);
