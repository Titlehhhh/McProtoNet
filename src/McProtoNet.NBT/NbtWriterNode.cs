namespace McProtoNet.NBT;

/// <summary>Holds the saved state of one node of the NBT tree while a writer descends into it.</summary>
internal sealed class NbtWriterNode
{
    public int ListIndex;
    public int ListSize;
    public NbtTagType ListType;
    public NbtTagType ParentType;
}