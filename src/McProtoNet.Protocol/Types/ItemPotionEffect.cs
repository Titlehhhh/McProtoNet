using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemPotionEffect
{
    public int Id { get; }
    public ItemEffectDetail Details { get; }

    public ItemPotionEffect(int id, ItemEffectDetail details)
    {
        Id = id;
        Details = details;
    }
}
