using McProtoNet.Geometry;

namespace McProtoNet.API.World.Implements
{
    public sealed class UnkownChunk : IChunk
    {

        public Point2_Int Position => new Point2_Int(0);
        IBlock[,,] blocks;
        public IBlock[,,] Blocks => blocks ?? (blocks = new IBlock[16, 16, 16]);

        public int SizeX => 16;

        public int SizeY => 16;

        public int SizeZ => 16;

        public IBlock GetBlock(int x, int y, int z)
        {
            return new UnkownBlock();
        }

        public IBlock GetBlock(Point3_Int position)
        {
            return GetBlock(position.X, position.Y, position.Z);
        }

        public IBlock GetBlock(Point3 player)
        {
            return GetBlock(player.ChunkX, player.ChunkY, player.ChunkZ);
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {

        }

        public void SetBlock(Point3_Int position, IBlock block)
        {

        }
    }
}
