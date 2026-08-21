using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Tag : IProtocolType<Tag>
{
    public string TagName { get; }
    public int[] Entries { get; }

    public Tag(string tagName, int[] entries)
    {
        TagName = tagName;
        Entries = entries;
    }

    public static Tag Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Tag>(protocolVersion);
        var tagName = reader.ReadString();
        int entriesCount = reader.ReadVarInt();
        var entries = new int[entriesCount];
        for (int i = 0; i < entries.Length; i++)
            entries[i] = reader.ReadVarInt();
        return new Tag(tagName, entries);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Tag>(protocolVersion);
        writer.WriteString(TagName);
        writer.WriteVarInt(Entries.Length);
        foreach (var entriesItem in Entries)
            writer.WriteVarInt(entriesItem);
    }
}
