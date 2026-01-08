using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemSoundHolder
{
    public bool HasInline { get; }
    public int? RegistryId { get; }
    public ItemSoundEvent? Inline { get; }

    public ItemSoundHolder(int registryId)
    {
        HasInline = false;
        RegistryId = registryId;
        Inline = null;
    }

    public ItemSoundHolder(ItemSoundEvent inline)
    {
        HasInline = true;
        RegistryId = null;
        Inline = inline;
    }
}
