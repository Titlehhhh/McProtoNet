namespace McProtoNet.API.World
{
    public interface IChunkColumn
    {
        int X { get; }
        int Z { get; }
        int SizeY { get; }
        IChunk[] Chunks { get; }
        #region Get/Set IChunk        
        IChunk GetChunk(int y);
        #endregion


    }
}
