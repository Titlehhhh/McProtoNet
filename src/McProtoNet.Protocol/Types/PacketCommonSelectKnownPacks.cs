using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonSelectKnownPacks
{
    public PackEntry[] Packs { get; }

    public PacketCommonSelectKnownPacks(PackEntry[] packs)
    {
        Packs = packs;
    }

    public sealed record PackEntry(string Namespace, string Id, string Version);
}
