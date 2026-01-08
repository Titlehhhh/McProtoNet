using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCustomClickAction
{
    public string Id { get; }
    public NbtTag? Nbt { get; }

    public PacketCommonCustomClickAction(string id, NbtTag? nbt)
    {
        Id = id;
        Nbt = nbt;
    }
}
