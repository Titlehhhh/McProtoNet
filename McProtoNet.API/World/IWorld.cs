using McProtoNet.Geometry;


namespace McProtoNet.API.World
{
    public interface IWorld : IDisposable
    {
        #region Get/Set IChunkColumn

        IChunkColumn GetChunkColumn(Point2_Int postition);
        IChunkColumn GetChunkColumn(int x, int z);
        IChunkColumn GetChunkColumn(Point3 player);
        void SetChunkColumn(Point2_Int position, IChunkColumn column);
        void SetChunkColumn(int x, int z, IChunkColumn column);
        #endregion

        #region Get/Set IChunk
        IChunk GetChunk(Point3_Int position);
        IChunk GetChunk(int x, int y, int z);
        IChunk GetChunk(Point3 player);

        #endregion
        #region Get/Set IBlock
        IBlock GetBlock(Point3_Int position);
        IBlock GetBlock(int x, int y, int z);
        IBlock GetBlock(Point3 player);

        void SetBlock(Point3_Int position, IBlock block);
        void SetBlock(int x, int y, int z, IBlock block);
        #endregion

        #region Свойства
        Dictionary<long, IChunkColumn> ChunkColumns { get; }
        #endregion

        #region Events
        event EventHandler<BlockChangeEventArgs> ChangeBlock;
        event EventHandler<ChunkColumnRecord> LoadChunkColumn;
        event EventHandler<ChunkColumnRecord> UnLoadChunkColumn;
        #endregion

        #region Методы
        void Clear();
        void UnLoadChunk(int x, int z);
        #endregion
        #region Удобные функции
        IEnumerable<IBlock> FindBlock(Point3_Int start, double radius, int id);
        IEnumerable<IBlock> FindBlock(Point3_Int start, double radius, int id, byte meta = 0);
        #endregion

    }
}
