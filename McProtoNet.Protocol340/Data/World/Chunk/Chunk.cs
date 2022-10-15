namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class Chunk
    {
        private readonly Block[,,] blocks;

        public Chunk(Block[,,] blocks)
        {
            this.blocks = blocks;
        }
        public Chunk(int size)
        {
            this.blocks = new Block[size,size,size];
        }

        public Block this[int x, int y, int z]
        {
            get
            {
                return blocks[x,y,z];
            }
            set
            {
                blocks[x, y, z] = value;
            }
        }
    }
}
