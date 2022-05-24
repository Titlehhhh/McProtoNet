namespace McProtoNet.API.World
{
    public sealed class ChunkColumn : IChunkColumn
    {


        private int x;
        private int z;
        public int X => x;

        public int Z => z;
        IChunk[] chunks;

        public IChunk[] Chunks => chunks ?? (chunks = new Chunk[16]);

        public int SizeY { get; private set; }

        public IChunk GetChunk(int y)
        {
            if (y >= 0 && y < SizeY)
                return Chunks[y];
            return new UnkownChunk();
        }

        public ChunkColumn(int x, int z, int sizeY = 16)
        {
            chunks = new Chunk[sizeY];
            this.SizeY = sizeY;
            this.x = x;
            this.z = z;
        }

    }
}
