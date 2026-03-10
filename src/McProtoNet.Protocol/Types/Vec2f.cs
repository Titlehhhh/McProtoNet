using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Vec2f(float X, float Y);
