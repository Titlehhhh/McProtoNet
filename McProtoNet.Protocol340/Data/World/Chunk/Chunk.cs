namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class Chunk
    {
        private readonly Block[] blocks;

        public Chunk(Block[] blocks)
        {
            this.blocks = blocks;
        }
        public Chunk(int size)
        {
            this.blocks = new Block[size * size * size];
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                return blocks[(y << 8) | (z << 4) | x];
            }
            set
            {
                blocks[(y << 8) | (z << 4) | x] = value;
            }
        }
    }
}
