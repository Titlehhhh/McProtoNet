using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(762, 772)]
public readonly partial record struct Vec4f(float x, float y, float z, float w);
