using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Tag
{
    public string TagName { get; }
    public int[] Entries { get; }

    public Tag(string tagName, int[] entries)
    {
        TagName = tagName;
        Entries = entries;
    }
}
