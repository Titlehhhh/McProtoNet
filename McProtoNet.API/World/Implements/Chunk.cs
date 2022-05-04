using McProtoNet.Geometry;

namespace McProtoNet.API.World.Implements
{
    public sealed class Chunk : IChunk
    {
        private readonly IBlock[,,] blocks;

        public IBlock[,,] Blocks => blocks;

        public int SizeX { get; private set; }

        public int SizeY { get; private set; }

        public int SizeZ { get; private set; }

        //private static IBlock UnkownBlock = new IBlock(0, 0, new Point3_Int(0));
        public IBlock GetBlock(int x, int y, int z)
        {
            if (IsValidPos(x, SizeX - 1) && IsValidPos(y, SizeY - 1) && IsValidPos(z, SizeZ - 1))
                return Blocks[x, y, z];
            return new UnkownBlock();
        }
        private static bool IsValidPos(int value, int max)
        {
            return value >= 0 && value <= max;
        }

        public IBlock GetBlock(Point3_Int position)
        {
            return GetBlock(position.X, position.Y, position.Z);
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {
            if (block is null)
                throw new ArgumentNullException(nameof(block));
            if (IsValidPos(x, SizeX - 1) && IsValidPos(y, SizeY - 1) && IsValidPos(z, SizeZ - 1))
                Blocks[x, y, z] = block;
        }

        public void SetBlock(Point3_Int position, IBlock block)
        {
            SetBlock(position.X, position.Y, position.Z, block);
        }

        public Chunk(int sizeX = 16, int sizeY = 16, int sizeZ = 16)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
            blocks = new IBlock[SizeX, SizeY, SizeZ];
        }
    }
}
