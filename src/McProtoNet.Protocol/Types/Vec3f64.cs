using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Vec3f64(double X, double Y, double Z);
