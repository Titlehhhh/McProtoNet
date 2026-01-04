using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class EntityMetadataPaintingVariant
{
    public int Width { get; }
    public int Height { get; }
    public string AssetId { get; }
    public NbtTag? Title { get; }
    public NbtTag? Author { get; }

    public EntityMetadataPaintingVariant(int width, int height, string assetId, NbtTag? title, NbtTag? author)
    {
        Width = width;
        Height = height;
        AssetId = assetId;
        Title = title;
        Author = author;
    }
}
