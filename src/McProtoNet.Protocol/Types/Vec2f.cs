using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, 772)]
public readonly partial record struct Vec2f(float x, float y);
