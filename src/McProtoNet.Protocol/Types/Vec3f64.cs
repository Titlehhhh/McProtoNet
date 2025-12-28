using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(762, 772)]
public readonly partial record struct Vec3f64(double x, double y, double z);
