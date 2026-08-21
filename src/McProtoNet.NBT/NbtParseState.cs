namespace McProtoNet.NBT;

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