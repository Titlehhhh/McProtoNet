using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public sealed partial class GameProfile
{
    public string Name { get; }
    public GameProfileProperty[] Properties { get; }

    public GameProfile(string name, GameProfileProperty[] properties)
    {
        Name = name;
        Properties = properties;
    }

    public sealed record GameProfileProperty(string Name, string Value, string? Signature);
}
