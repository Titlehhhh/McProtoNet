using McProtoNet.Geometry;

namespace McProtoNet.World.Implements
{
    public sealed class UnkownBlock : IBlock
    {
        public Point3_Int Position { get; set; } = new Point3_Int(0, 0, 0);

        public int ID { get; set; }
    }
}
