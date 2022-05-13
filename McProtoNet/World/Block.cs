using McProtoNet.Geometry;

namespace McProtoNet.World
{
    public interface IBlock
    {
        int ID { get; set; }
        Point3_Int Position { get; set; }

    }
}
