using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public readonly partial record struct MovementFlags(bool OnGround, bool HasHorizontalCollision);
