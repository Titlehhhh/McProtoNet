using McProtoNet.Geometry;

namespace McProtoNet.API.World
{
    public interface IBlock
    {
        int ID { get; set; }
        Point3_Int Position { get; set; }

    }
}
