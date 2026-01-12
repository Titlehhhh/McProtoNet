using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonClearDialog
{
}
