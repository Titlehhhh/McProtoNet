using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemBookPage
{
    public string Content { get; }
    public string? FilteredContent { get; }

    public ItemBookPage(string content, string? filteredContent)
    {
        Content = content;
        FilteredContent = filteredContent;
    }
}
