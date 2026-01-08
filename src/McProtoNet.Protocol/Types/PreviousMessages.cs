using System;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
public sealed partial class PreviousMessages
{
    public PreviousMessage[] Messages { get; }

    public PreviousMessages(PreviousMessage[] messages)
    {
        Messages = messages;
    }

    public sealed record PreviousMessage(Guid? MessageSender, byte[]? MessageSignature, int? Id, byte[]? Signature);
}
