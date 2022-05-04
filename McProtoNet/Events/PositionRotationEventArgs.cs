using McProtoNet.Geometry;

namespace McProtoNet
{
    public class PositionRotationEventArgs
    {
        public Point3 Position { get; private set; }
        public Rotation Rotation { get; private set; }

        public bool OnGround { get; private set; }

        public PositionRotationEventArgs(Point3 position, Rotation rotation, bool onGround)
        {
            Position = position;
            Rotation = rotation;
            OnGround = onGround;
        }
    }
}
