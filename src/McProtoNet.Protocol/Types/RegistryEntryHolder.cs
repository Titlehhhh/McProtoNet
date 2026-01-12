using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial record struct RegistryEntryHolder<T>
{
    public bool HasInline { get; }
    public int? RegistryId { get; }
    public T? Inline { get; }

    public RegistryEntryHolder(int registryId)
    {
        HasInline = false;
        RegistryId = registryId;
        Inline = default;
    }

    public RegistryEntryHolder(T inline)
    {
        HasInline = true;
        RegistryId = null;
        Inline = inline;
    }
}
