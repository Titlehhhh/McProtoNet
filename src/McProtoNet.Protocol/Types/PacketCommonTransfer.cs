using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonTransfer
{
    public string Host { get; }
    public int Port { get; }

    public PacketCommonTransfer(string host, int port)
    {
        Host = host;
        Port = port;
    }
}
