using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonServerLinks
{
    public ServerLinkEntry[] Links { get; }

    public PacketCommonServerLinks(ServerLinkEntry[] links)
    {
        Links = links;
    }

    public sealed record ServerLinkEntry(bool HasKnownType, ServerLinkType? KnownType, NbtTag? UnknownType, string Link);
}
