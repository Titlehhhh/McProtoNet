using McProtoNet.Geometry;

namespace McProtoNet.World
{
    public interface IChunk
    {
        int SizeX { get; }
        int SizeY { get; }
        int SizeZ { get; }
        #region Свойства              
        IBlock[,,] Blocks { get; }
        #endregion

        #region Get/Set IBlock
        IBlock GetBlock(int x, int y, int z);
        IBlock GetBlock(Point3_Int position);
        void SetBlock(int x, int y, int z, IBlock block);
        void SetBlock(Point3_Int position, IBlock block);
        #endregion

    }
}
