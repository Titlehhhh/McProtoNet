using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCustomReportDetails
{
    public DetailEntry[] Details { get; }

    public PacketCommonCustomReportDetails(DetailEntry[] details)
    {
        Details = details;
    }

    public sealed record DetailEntry(string Key, string Value);
}
