using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class RecipeBookSetting
{
    public bool Open { get; }
    public bool Filtering { get; }

    public RecipeBookSetting(bool open, bool filtering)
    {
        Open = open;
        Filtering = filtering;
    }
}
