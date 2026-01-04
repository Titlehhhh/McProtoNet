using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class BannerPatternLayer
{
    public RegistryEntryHolder<BannerPattern>? Pattern { get; }
    public ItemSoundHolder? PatternSound { get; }
    public int ColorId { get; }

    public BannerPatternLayer(RegistryEntryHolder<BannerPattern>? pattern, ItemSoundHolder? patternSound, int colorId)
    {
        Pattern = pattern;
        PatternSound = patternSound;
        ColorId = colorId;
    }
}
