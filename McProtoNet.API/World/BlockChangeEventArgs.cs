using McProtoNet.Geometry;

namespace McProtoNet.API.World
{
    public class BlockChangeEventArgs : EventArgs
    {
        public BlockChangeEventArgs(Point3_Int pos, IBlock oldBlock, IBlock newBlock)
        {
            Position = pos;
            OldBlock = oldBlock;
            NewBlock = newBlock;
        }

        public Point3_Int Position { get; private set; }
        public IBlock OldBlock { get; private set; }
        public IBlock NewBlock { get; private set; }
    }
}
