namespace McProtoNet.Protocol;

[MCVer(1,2)]
public readonly partial record struct Position(int X, int Y, int Z);

//Generated
public partial record struct Position
{
    public static ProtocolRange SupportedVersions = new ProtocolRange(1, 2);
}