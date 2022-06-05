using System;

namespace McProtoNet.Geometry
{
    public struct Rotation
    {
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        public Vector3 Vector { get; private set; }

        public Rotation(Vector3 vector)
        {
            Vector = vector;
            double r = vector.Distance;
            Yaw = (float)(-Math.Atan2(vector.X, vector.Y) / Math.PI * 180);
            if (Yaw < 0)
                Yaw += 360;
            Pitch = (float)(-Math.Asin(vector.Y / r) / Math.PI * 180);
        }
        public Rotation(float yaw, float pitch)
        {
            this.Yaw = yaw;
            this.Pitch = pitch;
            Vector = new Vector3(-Math.Cos(Pitch) * Math.Sin(Yaw), -Math.Sin(pitch), Math.Cos(Pitch) * Math.Cos(Yaw));

        }
    }
}
