using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class ChatTypesHolder
{
    public bool HasInline { get; }
    public int? RegistryId { get; }
    public ChatTypes? Data { get; }

    public ChatTypesHolder(int registryId)
    {
        HasInline = false;
        RegistryId = registryId;
        Data = null;
    }

    public ChatTypesHolder(ChatTypes data)
    {
        HasInline = true;
        RegistryId = null;
        Data = data;
    }
}
