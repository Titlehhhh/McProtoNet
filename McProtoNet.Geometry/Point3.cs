namespace McProtoNet.Geometry
{

    public struct Point3
    {

        public double X;
        public double Y;
        public double Z;

        public static Point3 Zero
        {
            get
            {
                return new Point3(0, 0, 0);
            }
        }
        public Point3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Point3(double value)
        {
            X = Y = Z = value;
        }


        public Point3(int chunkX, int chunkZ, int blockX, int blockY, int blockZ)
        {
            X = chunkX * 16 + blockX;
            Y = blockY;
            Z = chunkZ * 16 + blockZ;
        }


        public int ChunkX
        {
            get
            {
                return (int)X >> 4;
            }
        }


        public int ChunkY
        {
            get
            {
                return (int)Y >> 4;
            }
        }


        public int ChunkZ
        {
            get
            {
                return (int)Z >> 4;
            }
        }


        public int ChunkBlockX
        {
            get
            {
                int ceil = (int)X;
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
                int ceil = (int)Z;
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


        public double DistanceSquared(Point3 location)
        {
            return ((X - location.X) * (X - location.X))
                 + ((Y - location.Y) * (Y - location.Y))
                 + ((Z - location.Z) * (Z - location.Z));
        }


        public double Distance(Point3 location)
        {
            return Math.Sqrt(DistanceSquared(location));
        }


        public Point3 EyesLocation()
        {
            return this + new Point3(0, 1.62, 0);
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Point3)
            {
                return ((int)this.X) == ((int)((Point3)obj).X)
                    && ((int)this.Y) == ((int)((Point3)obj).Y)
                    && ((int)this.Z) == ((int)((Point3)obj).Z);
            }
            return false;
        }


        public static bool operator ==(Point3 loc1, Point3 loc2)
        {
            if (loc1 == null && loc2 == null)
                return true;
            if (loc1 == null || loc2 == null)
                return false;
            return loc1.Equals(loc2);
        }

        public static bool operator !=(Point3 loc1, Point3 loc2)
        {
            if (loc1 == null && loc2 == null)
                return true;
            if (loc1 == null || loc2 == null)
                return false;
            return !loc1.Equals(loc2);
        }


        public static Point3 operator +(Point3 loc1, Point3 loc2)
        {
            return new Point3
            (
                loc1.X + loc2.X,
                loc1.Y + loc2.Y,
                loc1.Z + loc2.Z
            );
        }
        public static Point3 operator +(Point3 loc1, Vector3 vector)
        {
            return new Point3
            (
                loc1.X + vector.X,
                loc1.Y + vector.Y,
                loc1.Z + vector.Z
            );
        }
        public static Point3 operator -(Point3 loc1, Vector3 vector)
        {
            return new Point3
            (
                loc1.X - vector.X,
                loc1.Y - vector.Y,
                loc1.Z - vector.Z
            );
        }
        public static implicit operator Vector3(Point3 a)
        {
            return new Vector3(a.X, a.Y, a.Z);
        }


        public static Point3 operator -(Point3 loc1, Point3 loc2)
        {
            return new Point3
            (
                loc1.X - loc2.X,
                loc1.Y - loc2.Y,
                loc1.Z - loc2.Z
            );
        }


        public static Point3 operator *(Point3 loc, double val)
        {
            return new Point3
            (
                loc.X * val,
                loc.Y * val,
                loc.Z * val
            );
        }

        public static Point3 operator /(Point3 loc, double val)
        {
            return new Point3
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
