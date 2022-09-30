using McProtoNet.NBT;

namespace McProtoNet.Protocol340.Data.World.Chunk
{
    public class Column
    {
        private int x;
        private int z;
        private Chunk[] chunks;
        private byte[] biomeData;
        private NbtCompound[] tileEntities;

        private bool skylight;

        public Column(int x, int z, Chunk[] chunks, NbtCompound[]? tileEntities) :
            this(x, z, chunks, null, tileEntities)
        {

        }
        public Column(int x, int z, Chunk[] chunks, byte[]? biomeData, NbtCompound[]? tileEntities)
        {
            if (chunks.Length != 16)
            {
                throw new ArgumentException("Chunk array length must be 16.");
            }
            if (biomeData != null && biomeData.Length != 256)
            {
                throw new ArgumentException("Biome data array length must be 256.");
            }

            this.skylight = false;
            bool noSkylight = false;


            foreach (Chunk chunk in chunks)
            {
                if (chunk != null)
                {
                    if (chunk.SkyLight == null)
                    {
                        noSkylight = true;
                    }
                    else
                    {
                        this.skylight = true;
                    }
                }
            }

            if (noSkylight && this.skylight)
            {
                throw new ArgumentException("Either all chunks must have skylight values or none must have them.");
            }


            this.x = x;
            this.z = z;
            this.chunks = chunks;
            this.biomeData = biomeData;

            if (tileEntities != null)
                this.tileEntities = tileEntities;
            else
                this.tileEntities = new NbtCompound[0];

            this.skylight = skylight;
        }


        public int X => this.x;


        public int Z => this.z;


        public Chunk[] Chunks => this.chunks;


        public bool HasBiomeData => this.biomeData != null;



        public byte[] BiomeData() => this.biomeData;

        public NbtCompound[] TileEntities => this.tileEntities;


        public bool HasSkyLight => skylight;



    }
}
