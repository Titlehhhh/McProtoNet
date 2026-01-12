using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemWrittenBookPage
{
    public NbtTag Content { get; }
    public NbtTag? FilteredContent { get; }

    public ItemWrittenBookPage(NbtTag content, NbtTag? filteredContent)
    {
        Content = content;
        FilteredContent = filteredContent;
    }
}
