using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCookieRequest
{
    public string Cookie { get; }

    public PacketCommonCookieRequest(string cookie)
    {
        Cookie = cookie;
    }
}
