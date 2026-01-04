using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonRemoveResourcePack
{
    public Guid? Uuid { get; }

    public PacketCommonRemoveResourcePack(Guid? uuid)
    {
        Uuid = uuid;
    }
}
