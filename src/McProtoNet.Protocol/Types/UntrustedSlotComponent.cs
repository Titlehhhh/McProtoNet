using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public sealed partial record UntrustedSlotComponent(SlotComponentType Type, byte[] Data)
{
}
