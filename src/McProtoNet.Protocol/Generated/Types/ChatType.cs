using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ChatType : IProtocolType<ChatType>
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

    public static ChatType Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatType>(protocolVersion);
        var translationKey = reader.ReadString();
        int parametersCount = reader.ReadVarInt();
        var parameters = new ChatTypeParameterType[parametersCount];
        for (int i = 0; i < parameters.Length; i++)
            parameters[i] = reader.ReadType<ChatTypeParameterType>(protocolVersion);
        var style = reader.ReadNbtTag(false)!;
        return new ChatType(translationKey, parameters, style);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatType>(protocolVersion);
        writer.WriteString(TranslationKey);
        writer.WriteVarInt(Parameters.Length);
        foreach (var parametersItem in Parameters)
            writer.WriteType<ChatTypeParameterType>(parametersItem, protocolVersion);
        writer.WriteNbt(Style);
    }
}
