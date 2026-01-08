using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonAddResourcePack
{
    public Guid Uuid { get; }
    public string Url { get; }
    public string Hash { get; }
    public bool Forced { get; }
    public NbtTag? PromptMessage { get; }

    public PacketCommonAddResourcePack(Guid uuid, string url, string hash, bool forced, NbtTag? promptMessage)
    {
        Uuid = uuid;
        Url = url;
        Hash = hash;
        Forced = forced;
        PromptMessage = promptMessage;
    }
}
