using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(770, 772)]
public readonly partial record struct Vec3i(int x, int y, int z);
