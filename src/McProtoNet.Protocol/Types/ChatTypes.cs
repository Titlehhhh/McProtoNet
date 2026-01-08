using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ChatTypes
{
    public ChatType Chat { get; }
    public ChatType Narration { get; }

    public ChatTypes(ChatType chat, ChatType narration)
    {
        Chat = chat;
        Narration = narration;
    }
}
