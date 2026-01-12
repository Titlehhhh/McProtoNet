using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCookieResponse
{
    public string Key { get; }
    public byte[]? Value { get; }

    public PacketCommonCookieResponse(string key, byte[]? value)
    {
        Key = key;
        Value = value;
    }
}
