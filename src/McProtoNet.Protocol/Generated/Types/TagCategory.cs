using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
public sealed partial class TagCategory : IProtocolType<TagCategory>
{
    public string TagType { get; }
    public Tag[] Tags { get; }

    public TagCategory(string tagType, Tag[] tags)
    {
        TagType = tagType;
        Tags = tags;
    }

    public static TagCategory Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TagCategory>(protocolVersion);
        var tagType = reader.ReadString();
        int tagsCount = reader.ReadVarInt();
        var tags = new Tag[tagsCount];
        for (int i = 0; i < tags.Length; i++)
            tags[i] = reader.ReadType<Tag>(protocolVersion);
        return new TagCategory(tagType, tags);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TagCategory>(protocolVersion);
        writer.WriteString(TagType);
        writer.WriteVarInt(Tags.Length);
        foreach (var tagsItem in Tags)
            writer.WriteType<Tag>(tagsItem, protocolVersion);
    }
}
