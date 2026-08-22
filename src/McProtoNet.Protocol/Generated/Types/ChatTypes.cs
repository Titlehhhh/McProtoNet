using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ChatTypes : IProtocolType<ChatTypes>
{
    public ChatType Chat { get; }
    public ChatType Narration { get; }

    public ChatTypes(ChatType chat, ChatType narration)
    {
        Chat = chat;
        Narration = narration;
    }

    public static ChatTypes Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypes>(protocolVersion);
        var chat = reader.ReadType<ChatType>(protocolVersion);
        var narration = reader.ReadType<ChatType>(protocolVersion);
        return new ChatTypes(chat, narration);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypes>(protocolVersion);
        writer.WriteType<ChatType>(Chat, protocolVersion);
        writer.WriteType<ChatType>(Narration, protocolVersion);
    }
}
