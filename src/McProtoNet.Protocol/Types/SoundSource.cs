using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public readonly partial record struct SoundSource(string Value);
