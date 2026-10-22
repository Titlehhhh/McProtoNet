namespace McProtoNet.NBT;

/// <summary>Holds the saved state of one node of the NBT tree while a reader descends into it.</summary>
internal sealed class NbtReaderNode
{
    public int ListIndex;
    public NbtTagType ListType;
    public string? ParentName;
    public int ParentTagLength;
    public NbtTagType ParentTagType;
}