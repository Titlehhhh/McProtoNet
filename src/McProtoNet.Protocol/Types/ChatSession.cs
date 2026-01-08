using System;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public sealed partial class ChatSession
{
    public Guid Uuid { get; }
    public ChatSessionPublicKey PublicKey { get; }

    public ChatSession(Guid uuid, ChatSessionPublicKey publicKey)
    {
        Uuid = uuid;
        PublicKey = publicKey;
    }

    public sealed record ChatSessionPublicKey(long ExpireTime, byte[] KeyBytes, byte[] KeySignature);
}
