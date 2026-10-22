using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public readonly partial record struct RecipeBookSetting(bool Open, bool Filtering) : IProtocolType<RecipeBookSetting>
{
    public static RecipeBookSetting Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookSetting>(protocolVersion);
        var open = reader.ReadBoolean();
        var filtering = reader.ReadBoolean();
        return new RecipeBookSetting(open, filtering);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RecipeBookSetting>(protocolVersion);
        writer.WriteBoolean(Open);
        writer.WriteBoolean(Filtering);
    }
}
