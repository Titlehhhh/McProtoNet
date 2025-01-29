namespace McProtoNet.Protocol;

public readonly struct Position
{
    public int X { get; }

    public int Y { get; }

    public int Z { get; }

    public Position(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
    {
        return $"X: {X}, Y: {Y}, Z:{Z}";
    }
}