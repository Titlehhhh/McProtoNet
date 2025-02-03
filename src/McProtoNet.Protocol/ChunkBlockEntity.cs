using McProtoNet.NBT;

namespace McProtoNet.Protocol;

public readonly struct ChunkBlockEntity
{
    public readonly byte X;
    public readonly byte Z;
    public readonly short Y;
    public readonly int Type;
    public readonly NbtTag? NbtData;

    public ChunkBlockEntity(byte x, byte z, short y, int type, NbtTag? nbtData)
    {
        X = x;
        Z = z;
        Y = y;
        Type = type;
        NbtData = nbtData;
    }
}