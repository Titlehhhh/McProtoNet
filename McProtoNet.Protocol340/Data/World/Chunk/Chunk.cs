namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class Chunk
    {
        private BlockStorage blocks;
        private NibbleArray3d blocklight;
        private NibbleArray3d skylight;

        public Chunk(bool skylight) : this(new BlockStorage(), new NibbleArray3d(4096), skylight ? new NibbleArray3d(4096) : null)
        {
            
        }

        public Chunk(BlockStorage blocks, NibbleArray3d blocklight, NibbleArray3d skylight)
        {
            this.blocks = blocks;
            this.blocklight = blocklight;
            this.skylight = skylight;
        }

        public BlockStorage Storage => this.blocks;
        public NibbleArray3d? BlockLight => this.blocklight;
        public NibbleArray3d? SkyLight => this.skylight;

       

        public bool IsEmpty => blocks.IsEmpty;
       
    }
}
