using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial record struct PositionUpdateRelatives(bool X, bool Y, bool Z, bool Yaw, bool Pitch,
    bool Dx, bool Dy, bool Dz, bool YawDelta);
