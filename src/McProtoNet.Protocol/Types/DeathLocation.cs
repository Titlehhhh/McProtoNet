using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial struct DeathLocation(string dimensionName, Position location)
{
    public readonly string DimensionName = dimensionName;
    public readonly Position Location = location;
}
