namespace McProtoNet.Geometry
{
    public struct Point3F
    {

        public float X;
        public float Y;
        public float Z;

        public static Point3F Zero
        {
            get
            {
                return new Point3F(0, 0, 0);
            }
        }
        public Point3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Point3F(float value)
        {
            X = Y = Z = value;
        }


        public Point3F(int chunkX, int chunkZ, int blockX, int blockY, int blockZ)
        {
            X = chunkX * 16 + blockX;
            Y = blockY;
            Z = chunkZ * 16 + blockZ;
        }


        public int ChunkX
        {
            get
            {
                return (int)Math.Floor(X / 16);
            }
        }


        public int ChunkY
        {
            get
            {
                return (int)Math.Floor(Y / 16);
            }
        }


        public int ChunkZ
        {
            get
            {
                return (int)Math.Floor(Z / 16);
            }
        }


        public int ChunkBlockX
        {
            get
            {
                int ceil = (int)Y;
                return ceil & 15;
            }
        }


        public int ChunkBlockY
        {
            get
            {
                int ceil = (int)Y;
                return ceil & 15;
            }
        }


        public int ChunkBlockZ
        {
            get
            {
                int ceil = (int)Y;
                return ceil & 15;
            }
        }
        public Point3_Int ChunkPos
        {
            get => new Point3_Int(ChunkX, ChunkY, ChunkZ);
        }
        public Point3_Int ChunkBlockPos
        {
            get => new Point3_Int(ChunkBlockX, ChunkBlockY, ChunkBlockZ);
        }


        public float DistanceSquared(Point3F location)
        {
            return ((X - location.X) * (X - location.X))
                 + ((Y - location.Y) * (Y - location.Y))
                 + ((Z - location.Z) * (Z - location.Z));
        }


        public float Distance(Point3F location)
        {
            return MathF.Sqrt(DistanceSquared(location));
        }


        public Point3F EyesLocation()
        {
            return this + new Point3F(0, 1.62f, 0);
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Point3F)
            {
                return ((int)this.X) == ((int)((Point3F)obj).X)
                    && ((int)this.Y) == ((int)((Point3F)obj).Y)
                    && ((int)this.Z) == ((int)((Point3F)obj).Z);
            }
            return false;
        }


        public static bool operator ==(Point3F loc1, Point3F loc2)
        {
            if (loc1 == null && loc2 == null)
                return true;
            if (loc1 == null || loc2 == null)
                return false;
            return loc1.Equals(loc2);
        }

        public static bool operator !=(Point3F loc1, Point3F loc2)
        {
            if (loc1 == null && loc2 == null)
                return true;
            if (loc1 == null || loc2 == null)
                return false;
            return !loc1.Equals(loc2);
        }


        public static Point3F operator +(Point3F loc1, Point3F loc2)
        {
            return new Point3F
            (
                loc1.X + loc2.X,
                loc1.Y + loc2.Y,
                loc1.Z + loc2.Z
            );
        }
        public static Point3F operator +(Point3F loc1, Vector3F vector)
        {
            return new Point3F
            (
                loc1.X + vector.X,
                loc1.Y + vector.Y,
                loc1.Z + vector.Z
            );
        }
        public static Point3F operator -(Point3F loc1, Vector3F vector)
        {
            return new Point3F
            (
                loc1.X - vector.X,
                loc1.Y - vector.Y,
                loc1.Z - vector.Z
            );
        }


        public static Point3F operator -(Point3F loc1, Point3F loc2)
        {
            return new Point3F
            (
                loc1.X - loc2.X,
                loc1.Y - loc2.Y,
                loc1.Z - loc2.Z
            );
        }


        public static Point3F operator *(Point3F loc, float val)
        {
            return new Point3F
            (
                loc.X * val,
                loc.Y * val,
                loc.Z * val
            );
        }

        public static Point3F operator /(Point3F loc, float val)
        {
            return new Point3F
            (
                loc.X / val,
                loc.Y / val,
                loc.Z / val
            );
        }


        public override int GetHashCode()
        {
            return (((int)X) & ~((~0) << 13)) << 19
                 | (((int)Y) & ~((~0) << 13)) << 13
                 | (((int)Z) & ~((~0) << 06)) << 00;
        }

        public override string ToString()
        {
            return String.Format("X:{0} Y:{1} Z:{2}", X, Y, Z);
        }
    }
}
