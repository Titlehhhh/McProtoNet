using System;

namespace McProtoNet.Geometry
{
    /// <summary>
    /// Represents a location into a Minecraft world
    /// </summary>
    public struct Point3
    {
        /// <summary>
        /// The X Coordinate
        /// </summary>
        public double X;

        /// <summary>
        /// The Y Coordinate (vertical)
        /// </summary>
        public double Y;

        /// <summary>
        /// The Z coordinate
        /// </summary>
        public double Z;

        /// <summary>
        /// Get location with zeroed coordinates
        /// </summary>
        public static Point3 Zero
        {
            get
            {
                return new Point3(0, 0, 0);
            }
        }

        /// <summary>
        /// Create a new location
        /// </summary>
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

        /// <summary>
        /// Create a new location
        /// </summary>
        /// <param name="chunkX">Location of the chunk into the world</param>
        /// <param name="chunkZ">Location of the chunk into the world</param>
        /// <param name="blockX">Location of the block into the chunk</param>
        /// <param name="blockY">Location of the block into the world</param>
        /// <param name="blockZ">Location of the block into the chunk</param>
        public Point3(int chunkX, int chunkZ, int blockX, int blockY, int blockZ)
        {
            X = chunkX * 16 + blockX;
            Y = blockY;
            Z = chunkZ * 16 + blockZ;
        }

        /// <summary>
        /// The X index of the corresponding chunk in the world
        /// </summary>
        public int ChunkX
        {
            get
            {
                return (int)Math.Floor(X / 16);
            }
        }

        /// <summary>
        /// The Y index of the corresponding chunk in the world
        /// </summary>
        public int ChunkY
        {
            get
            {
                return (int)Math.Floor(Y / 16);
            }
        }

        /// <summary>
        /// The Z index of the corresponding chunk in the world
        /// </summary>
        public int ChunkZ
        {
            get
            {
                return (int)Math.Floor(Z / 16);
            }
        }

        /// <summary>
        /// The X index of the corresponding block in the corresponding chunk of the world
        /// </summary>
        public int ChunkBlockX
        {
            get
            {
                int ceil = (int)Y;
                return ceil & 15;
            }
        }

        /// <summary>
        /// The Y index of the corresponding block in the corresponding chunk of the world
        /// </summary>
        public int ChunkBlockY
        {
            get
            {
                int ceil = (int)Y;
                return ceil & 15;
            }
        }

        /// <summary>
        /// The Z index of the corresponding block in the corresponding chunk of the world
        /// </summary>
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

        /// <summary>
        /// Get a squared distance to the specified location
        /// </summary>
        /// <param name="location">Other location for computing distance</param>
        /// <returns>Distance to the specified location, without using a square root</returns>
        public double DistanceSquared(Point3 location)
        {
            return ((X - location.X) * (X - location.X))
                 + ((Y - location.Y) * (Y - location.Y))
                 + ((Z - location.Z) * (Z - location.Z));
        }

        /// <summary>
        /// Get exact distance to the specified location
        /// </summary>
        /// <param name="location">Other location for computing distance</param>
        /// <returns>Distance to the specified location, with square root so lower performances</returns>
        public double Distance(Point3 location)
        {
            return Math.Sqrt(DistanceSquared(location));
        }

        /// <summary>
        /// Considering the current location as Feet location, calculate Eyes location
        /// </summary>
        /// <returns>Player Eyes location</returns>
        public Point3 EyesLocation()
        {
            return this + new Point3(0, 1.62, 0);
        }

        /// <summary>
        /// Compare two locations. Locations are equals if the integer part of their coordinates are equals.
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>TRUE if the locations are equals</returns>
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

        /// <summary>
        /// Compare two locations. Locations are equals if the integer part of their coordinates are equals.
        /// </summary>
        /// <param name="loc1">First location to compare</param>
        /// <param name="loc2">Second location to compare</param>
        /// <returns>TRUE if the locations are equals</returns>
        public static bool operator ==(Point3 loc1, Point3 loc2)
        {
            if (loc1 == null && loc2 == null)
                return true;
            if (loc1 == null || loc2 == null)
                return false;
            return loc1.Equals(loc2);
        }

        /// <summary>
        /// Compare two locations. Locations are not equals if the integer part of their coordinates are not equals.
        /// </summary>
        /// <param name="loc1">First location to compare</param>
        /// <param name="loc2">Second location to compare</param>
        /// <returns>TRUE if the locations are equals</returns>
        public static bool operator !=(Point3 loc1, Point3 loc2)
        {
            if (loc1 == null && loc2 == null)
                return true;
            if (loc1 == null || loc2 == null)
                return false;
            return !loc1.Equals(loc2);
        }

        /// <summary>
        /// Sums two locations and returns the result.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Thrown if one of the provided location is null
        /// </exception>
        /// <param name="loc1">First location to sum</param>
        /// <param name="loc2">Second location to sum</param>
        /// <returns>Sum of the two locations</returns>
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

        /// <summary>
        /// Substract a location to another
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Thrown if one of the provided location is null
        /// </exception>
        /// <param name="loc1">First location</param>
        /// <param name="loc2">Location to substract to the first one</param>
        /// <returns>Sum of the two locations</returns>
        public static Point3 operator -(Point3 loc1, Point3 loc2)
        {
            return new Point3
            (
                loc1.X - loc2.X,
                loc1.Y - loc2.Y,
                loc1.Z - loc2.Z
            );
        }

        /// <summary>
        /// Multiply a location by a scalar value
        /// </summary>
        /// <param name="loc">Location to multiply</param>
        /// <param name="val">Scalar value</param>
        /// <returns>Product of the location and the scalar value</returns>
        public static Point3 operator *(Point3 loc, double val)
        {
            return new Point3
            (
                loc.X * val,
                loc.Y * val,
                loc.Z * val
            );
        }

        /// <summary>
        /// Divide a location by a scalar value
        /// </summary>
        /// <param name="loc">Location to divide</param>
        /// <param name="val">Scalar value</param>
        /// <returns>Result of the division</returns>
        public static Point3 operator /(Point3 loc, double val)
        {
            return new Point3
            (
                loc.X / val,
                loc.Y / val,
                loc.Z / val
            );
        }

        /// <summary>
        /// DO NOT USE. Defined to comply with C# requirements requiring a GetHashCode() when overriding Equals() or ==
        /// </summary>
        /// <remarks>
        /// A modulo will be applied if the location is outside the following ranges:
        /// X: -4096 to +4095
        /// Y: -32 to +31
        /// Z: -4096 to +4095
        /// </remarks>
        /// <returns>A simplified version of the location</returns>
        public override int GetHashCode()
        {
            return (((int)X) & ~((~0) << 13)) << 19
                 | (((int)Y) & ~((~0) << 13)) << 13
                 | (((int)Z) & ~((~0) << 06)) << 00;
        }

        /// <summary>
        /// Convert the location into a string representation
        /// </summary>
        /// <returns>String representation of the location</returns>
        public override string ToString()
        {
            return String.Format("X:{0} Y:{1} Z:{2}", X, Y, Z);
        }
    }
}
