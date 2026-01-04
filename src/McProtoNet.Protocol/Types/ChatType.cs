using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ChatType
{
    public string TranslationKey { get; }
    public ChatTypeParameterType[] Parameters { get; }
    public NbtTag Style { get; }

    public ChatType(string translationKey, ChatTypeParameterType[] parameters, NbtTag style)
    {
        TranslationKey = translationKey;
        Parameters = parameters;
        Style = style;
    }
}
