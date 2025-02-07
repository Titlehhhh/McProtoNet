namespace McProtoNet.Protocol;

public readonly struct DeathLocation(string dimensionName, Position location)
{
    public readonly string DimensionName = dimensionName;
    public readonly Position Location = location;
}