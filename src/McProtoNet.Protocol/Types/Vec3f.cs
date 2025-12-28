using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Vec3f(float X, float Y, float Z);
