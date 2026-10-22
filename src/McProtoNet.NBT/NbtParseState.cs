namespace McProtoNet.NBT;

/// <summary>Specifies the position of an NBT reader within the document it is parsing.</summary>
internal enum NbtParseState
{
    AtStreamBeginning,
    AtCompoundBeginning,
    InCompound,
    AtCompoundEnd,
    AtListBeginning,
    InList,
    AtRootValue,
    AtStreamEnd,
    Error
}