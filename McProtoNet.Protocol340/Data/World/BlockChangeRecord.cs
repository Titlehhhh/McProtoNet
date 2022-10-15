using McProtoNet.Protocol340.Data.World.Chunk;

namespace McProtoNet.Protocol340.Data
{
    public record class BlockChangeRecord(Point3_Int Position, Block State);

}
