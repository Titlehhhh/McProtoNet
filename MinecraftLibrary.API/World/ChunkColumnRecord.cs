namespace ProtoLib.API.World
{
    public struct ChunkColumnRecord
    {
        public int X { get; private set; }
        public int Z { get; private set; }

        public ChunkColumnRecord(int x, int z)
        {
            X = x;
            Z = z;
        }
    }
}
