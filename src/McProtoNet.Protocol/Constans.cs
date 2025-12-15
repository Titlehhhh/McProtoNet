namespace McProtoNet.Protocol;

public static class Constants 
{
    public static readonly ProtocolRange AllVersions = 
        new(MinecraftVersion.StartVersion, MinecraftVersion.Latest);
}