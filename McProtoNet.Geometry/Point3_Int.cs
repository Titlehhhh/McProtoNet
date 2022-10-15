namespace McProtoNet.Geometry
{
    /// <summary>
    /// Represents a tuple of 3D coordinates.
    /// </summary>
    public struct Point3_Int : IEquatable<Point3_Int>
    {

        /// <summary>
        /// The X component of the coordinates.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the coordinates.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z component of the coordinates.
        /// </summary>
        public int Z;


        /// <summary>
        /// Creates a new trio of coordinates from the specified value.
        /// </summary>
        /// <param name="value">The value for the components of the coordinates.</param>
        public Point3_Int(int value)
        {
            X = Y = Z = value;
        }


        /// <summary>
        /// Creates a new trio of coordinates from the specified values.
        /// </summary>
        /// <param name="x">The X component of the coordinates.</param>
        /// <param name="z">The Y component of the coordinates.</param>
        /// <param name="z">The Z component of the coordinates.</param>
        public Point3_Int(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a new trio of coordinates by copying another.
        /// </summary>
        /// <param name="v">The coordinates to copy.</param>
        public Point3_Int(Point3_Int v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Converts this Coordinates3D to a string in the format &lt;x, y, z&gt;.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", X, Y, Z);
        }

        #region Math


        public Point3_Int ChunkBlock
        {
            get
            {
                return new Point3_Int(this.X & 15, this.Y & 15, this.Z & 15);
            }
        }
        public Point3_Int Chunk
        {
            get
            {
                return new Point3_Int(this.X >> 4, this.Y >> 4, this.Z >> 4);
            }
        }

        /// <summary>
        /// Clamps the coordinates to within the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Clamp(int value)
        {
            // TODO: Fix for negative values
            if (Math.Abs(X) > value)
                X = value * (X < 0 ? -1 : 1);
            if (Math.Abs(Y) > value)
                Y = value * (Y < 0 ? -1 : 1);
            if (Math.Abs(Z) > value)
                Z = value * (Z < 0 ? -1 : 1);
        }

        /// <summary>
        /// Calculates the distance between two Coordinates3D objects.
        /// </summary>
        public double DistanceTo(Point3_Int other)
        {
            return Math.Sqrt(Square(other.X - X) +
                             Square(other.Y - Y) +
                             Square(other.Z - Z));
        }

        /// <summary>
        /// Calculates the square of a num.
        /// </summary>
        private int Square(int num)
        {
            return num * num;
        }

        /// <summary>
        /// Finds the distance of this Coordinate3D from Coordinates3D.Zero
        /// </summary>
        public double Distance
        {
            get
            {
                return DistanceTo(Zero);
            }
        }

        /// <summary>
        /// Returns the component-wise minimum of two 3D coordinates.
        /// </summary>
        /// <param name="value1">The first coordinates.</param>
        /// <param name="value2">The second coordinates.</param>
        /// <returns></returns>
        public static Point3_Int Min(Point3_Int value1, Point3_Int value2)
        {
            return new Point3_Int(
                Math.Min(value1.X, value2.X),
                Math.Min(value1.Y, value2.Y),
                Math.Min(value1.Z, value2.Z)
                );
        }

        /// <summary>
        /// Returns the component-wise maximum of two 3D coordinates.
        /// </summary>
        /// <param name="value1">The first coordinates.</param>
        /// <param name="value2">The second coordinates.</param>
        /// <returns></returns>
        public static Point3_Int Max(Point3_Int value1, Point3_Int value2)
        {
            return new Point3_Int(
                Math.Max(value1.X, value2.X),
                Math.Max(value1.Y, value2.Y),
                Math.Max(value1.Z, value2.Z)
                );
        }

        #endregion

        #region Operators

        public static bool operator !=(Point3_Int a, Point3_Int b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Point3_Int a, Point3_Int b)
        {
            return a.Equals(b);
        }

        public static Point3_Int operator +(Point3_Int a, Point3_Int b)
        {
            return new Point3_Int(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3_Int operator -(Point3_Int a, Point3_Int b)
        {
            return new Point3_Int(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point3_Int operator -(Point3_Int a)
        {
            return new Point3_Int(-a.X, -a.Y, -a.Z);
        }

        public static Point3_Int operator *(Point3_Int a, Point3_Int b)
        {
            return new Point3_Int(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Point3_Int operator /(Point3_Int a, Point3_Int b)
        {
            return new Point3_Int(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Point3_Int operator %(Point3_Int a, Point3_Int b)
        {
            return new Point3_Int(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
        }

        public static Point3_Int operator +(Point3_Int a, int b)
        {
            return new Point3_Int(a.X + b, a.Y + b, a.Z + b);
        }

        public static Point3_Int operator -(Point3_Int a, int b)
        {
            return new Point3_Int(a.X - b, a.Y - b, a.Z - b);
        }

        public static Point3_Int operator *(Point3_Int a, int b)
        {
            return new Point3_Int(a.X * b, a.Y * b, a.Z * b);
        }

        public static Point3_Int operator /(Point3_Int a, int b)
        {
            return new Point3_Int(a.X / b, a.Y / b, a.Z / b);
        }

        public static Point3_Int operator %(Point3_Int a, int b)
        {
            return new Point3_Int(a.X % b, a.Y % b, a.Z % b);
        }


        public static Point3_Int operator +(int a, Point3_Int b)
        {
            return new Point3_Int(a + b.X, a + b.Y, a + b.Z);
        }

        public static Point3_Int operator -(int a, Point3_Int b)
        {
            return new Point3_Int(a - b.X, a - b.Y, a - b.Z);
        }

        public static Point3_Int operator *(int a, Point3_Int b)
        {
            return new Point3_Int(a * b.X, a * b.Y, a * b.Z);
        }

        public static Point3_Int operator /(int a, Point3_Int b)
        {
            return new Point3_Int(a / b.X, a / b.Y, a / b.Z);
        }

        public static Point3_Int operator %(int a, Point3_Int b)
        {
            return new Point3_Int(a % b.X, a % b.Y, a % b.Z);
        }


        #endregion

        #region Conversion operators

        public static explicit operator Point3_Int(Point2_Int a)
        {
            return new Point3_Int(a.X, 0, a.Z);
        }

        public static explicit operator Point3_Int(Vector3 a)
        {
            return new Point3_Int((int)a.X,
                                     (int)a.Y,
                                     (int)a.Z);
        }
        public static explicit operator Point3_Int(Point3 a)
        {
            return new Point3_Int((int)a.X,
                                     (int)a.Y,
                                     (int)a.Z);
        }



        #endregion

        #region Constants

        /// <summary>
        /// A trio of 3D coordinates with components set to 0.0.
        /// </summary>
        public static readonly Point3_Int Zero = new Point3_Int(0);

        /// <summary>
        /// A trio of 3D coordinates with components set to 0.0.
        /// </summary>
        public static readonly Point3_Int One = new Point3_Int(1);


        /// <summary>
        /// A trio of 3D coordinates facing up.
        /// </summary>
        public static readonly Point3_Int Up = new Point3_Int(0, 1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing down.
        /// </summary>
        public static readonly Point3_Int Down = new Point3_Int(0, -1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing left.
        /// </summary>
        public static readonly Point3_Int Left = new Point3_Int(-1, 0, 0);

        /// <summary>
        /// A trio of 3D coordinates facing right.
        /// </summary>
        public static readonly Point3_Int Right = new Point3_Int(1, 0, 0);

        /// <summary>
        /// A trio of 3D coordinates facing backwards.
        /// </summary>
        public static readonly Point3_Int Backwards = new Point3_Int(0, 0, -1);

        /// <summary>
        /// A trio of 3D coordinates facing forwards.
        /// </summary>
        public static readonly Point3_Int Forwards = new Point3_Int(0, 0, 1);


        /// <summary>
        /// A trio of 3D coordinates facing to the east.
        /// </summary>
        public static readonly Point3_Int East = new Point3_Int(1, 0, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the west.
        /// </summary>
        public static readonly Point3_Int West = new Point3_Int(-1, 0, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the north.
        /// </summary>
        public static readonly Point3_Int North = new Point3_Int(0, 0, -1);

        /// <summary>
        /// A trio of 3D coordinates facing to the south.
        /// </summary>
        public static readonly Point3_Int South = new Point3_Int(0, 0, 1);


        /// <summary>
        /// A trio of 3D coordinates facing the X axis at unit length.
        /// </summary>
        public static readonly Point3_Int OneX = new Point3_Int(1, 0, 0);

        /// <summary>
        /// A trio of 3D coordinates facing the Y axis at unit length.
        /// </summary>
        public static readonly Point3_Int OneY = new Point3_Int(0, 1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing the Z axis at unit length.
        /// </summary>
        public static readonly Point3_Int OneZ = new Point3_Int(0, 0, 1);

        #endregion

        /// <summary>
        /// Determines whether this 3D coordinates and another are equal.
        /// </summary>
        /// <param name="other">The other coordinates.</param>
        /// <returns></returns>
        public bool Equals(Point3_Int other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
        }

        /// <summary>
        /// Determines whether this and another object are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Point3_Int)) return false;
            return Equals((Point3_Int)obj);
        }

        /// <summary>
        /// Returns the hash code for this 3D coordinates.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }
    }
}
