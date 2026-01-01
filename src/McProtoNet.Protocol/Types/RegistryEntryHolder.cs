using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial record struct RegistryEntryHolder<T>
{
}
