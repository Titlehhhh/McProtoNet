using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class BannerPattern
{
    public string AssetId { get; }
    public string TranslationKey { get; }

    public BannerPattern(string assetId, string translationKey)
    {
        AssetId = assetId;
        TranslationKey = translationKey;
    }
}
