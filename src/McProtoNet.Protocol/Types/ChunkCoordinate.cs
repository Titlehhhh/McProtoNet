namespace McProtoNet.Protocol;

public readonly struct ChunkCoordinate(int x, int z, int y)
{
    public readonly int X = x;
    public readonly int Z = z;
    public readonly int Y = y;
}