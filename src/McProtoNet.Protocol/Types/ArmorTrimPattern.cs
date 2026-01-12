using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ArmorTrimPattern
{
    public string AssetId { get; }
    public int? TemplateItemId { get; }
    public NbtTag Description { get; }
    public bool Decal { get; }

    public ArmorTrimPattern(string assetId, int? templateItemId, NbtTag description, bool decal)
    {
        AssetId = assetId;
        TemplateItemId = templateItemId;
        Description = description;
        Decal = decal;
    }
}
