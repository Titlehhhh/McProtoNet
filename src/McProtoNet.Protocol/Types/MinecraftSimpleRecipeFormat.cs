using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, 767)]
public sealed partial class MinecraftSimpleRecipeFormat
{
    public int Category { get; }

    public MinecraftSimpleRecipeFormat(int category)
    {
        Category = category;
    }
}
