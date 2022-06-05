using System;

namespace McProtoNet.Geometry
{
    public struct Point2_Int : IEquatable<Point2_Int>
    {
        /// <summary>
        /// The X component of the coordinates.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y component of the coordinates.
        /// </summary>
        public int Z;

        /// <summary>
        /// Creates a new pair of coordinates from the specified value.
        /// </summary>
        /// <param name="value">The value for the components of the coordinates.</param>
        public Point2_Int(int value)
        {
            X = Z = value;
        }

        /// <summary>
        /// Creates a new pair of coordinates from the specified values.
        /// </summary>
        /// <param name="x">The X component of the coordinates.</param>
        /// <param name="z">The Y component of the coordinates.</param>
        public Point2_Int(int x, int z)
        {
            X = x;
            Z = z;
        }

        /// <summary>
        /// Creates a new pair of coordinates by copying another.
        /// </summary>
        /// <param name="v">The coordinates to copy.</param>
        public Point2_Int(Point2_Int v)
        {
            X = v.X;
            Z = v.Z;
        }

        /// <summary>
        /// Returns the string representation of this 2D coordinates.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<{0},{1}>", X, Z);
        }

        #region Math

        /// <summary>
        /// Calculates the distance between two Coordinates2D objects.
        /// </summary>
        public double DistanceTo(Point2_Int other)
        {
            return Math.Sqrt(Square(other.X - X) +
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
        /// Finds the distance of this Coordinates2D from Coordinates2D.Zero
        /// </summary>
        public double Distance
        {
            get
            {
                return DistanceTo(Zero);
            }
        }

        /// <summary>
        /// Returns the component-wise minimum of two 2D coordinates.
        /// </summary>
        /// <param name="value1">The first coordinates.</param>
        /// <param name="value2">The second coordinates.</param>
        /// <returns></returns>
        public static Point2_Int Min(Point2_Int value1, Point2_Int value2)
        {
            return new Point2_Int(
                Math.Min(value1.X, value2.X),
                Math.Min(value1.Z, value2.Z)
                );
        }

        /// <summary>
        /// Returns the component-wise maximum of two 2D coordinates.
        /// </summary>
        /// <param name="value1">The first coordinates.</param>
        /// <param name="value2">The second coordinates.</param>
        /// <returns></returns>
        public static Point2_Int Max(Point2_Int value1, Point2_Int value2)
        {
            return new Point2_Int(
                Math.Max(value1.X, value2.X),
                Math.Max(value1.Z, value2.Z)
                );
        }

        #endregion

        #region Operators

        public static bool operator !=(Point2_Int a, Point2_Int b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Point2_Int a, Point2_Int b)
        {
            return a.Equals(b);
        }

        public static Point2_Int operator +(Point2_Int a, Point2_Int b)
        {
            return new Point2_Int(a.X + b.X, a.Z + b.Z);
        }

        public static Point2_Int operator -(Point2_Int a, Point2_Int b)
        {
            return new Point2_Int(a.X - b.X, a.Z - b.Z);
        }

        public static Point2_Int operator -(Point2_Int a)
        {
            return new Point2_Int(
                -a.X,
                -a.Z);
        }

        public static Point2_Int operator *(Point2_Int a, Point2_Int b)
        {
            return new Point2_Int(a.X * b.X, a.Z * b.Z);
        }

        public static Point2_Int operator /(Point2_Int a, Point2_Int b)
        {
            return new Point2_Int(a.X / b.X, a.Z / b.Z);
        }

        public static Point2_Int operator %(Point2_Int a, Point2_Int b)
        {
            return new Point2_Int(a.X % b.X, a.Z % b.Z);
        }

        public static Point2_Int operator +(Point2_Int a, int b)
        {
            return new Point2_Int(a.X + b, a.Z + b);
        }

        public static Point2_Int operator -(Point2_Int a, int b)
        {
            return new Point2_Int(a.X - b, a.Z - b);
        }

        public static Point2_Int operator *(Point2_Int a, int b)
        {
            return new Point2_Int(a.X * b, a.Z * b);
        }

        public static Point2_Int operator /(Point2_Int a, int b)
        {
            return new Point2_Int(a.X / b, a.Z / b);
        }

        public static Point2_Int operator %(Point2_Int a, int b)
        {
            return new Point2_Int(a.X % b, a.Z % b);
        }

        public static Point2_Int operator +(int a, Point2_Int b)
        {
            return new Point2_Int(a + b.X, a + b.Z);
        }

        public static Point2_Int operator -(int a, Point2_Int b)
        {
            return new Point2_Int(a - b.X, a - b.Z);
        }

        public static Point2_Int operator *(int a, Point2_Int b)
        {
            return new Point2_Int(a * b.X, a * b.Z);
        }

        public static Point2_Int operator /(int a, Point2_Int b)
        {
            return new Point2_Int(a / b.X, a / b.Z);
        }

        public static Point2_Int operator %(int a, Point2_Int b)
        {
            return new Point2_Int(a % b.X, a % b.Z);
        }

        public static explicit operator Point2_Int(Point3_Int a)
        {
            return new Point2_Int(a.X, a.Z);
        }

        #endregion

        #region Constants

        /// <summary>
        /// A pair of 2D coordinates with components set to 0.0.
        /// </summary>
        public static readonly Point2_Int Zero = new Point2_Int(0);

        /// <summary>
        /// A pair of 2D coordinates with components set to 1.0.
        /// </summary>
        public static readonly Point2_Int One = new Point2_Int(1);


        /// <summary>
        /// A pair of 2D coordinates facing forwards.
        /// </summary>
        public static readonly Point2_Int Forward = new Point2_Int(0, 1);

        /// <summary>
        /// A pair of 2D coordinates facing backwards.
        /// </summary>
        public static readonly Point2_Int Backward = new Point2_Int(0, -1);

        /// <summary>
        /// A pair of 2D coordinates facing left.
        /// </summary>
        public static readonly Point2_Int Left = new Point2_Int(-1, 0);

        /// <summary>
        /// A pair of 2D coordinates facing right.
        /// </summary>
        public static readonly Point2_Int Right = new Point2_Int(1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the east.
        /// </summary>
        public static readonly Point2_Int East = new Point2_Int(1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the west.
        /// </summary>
        public static readonly Point2_Int West = new Point2_Int(-1, 0);

        /// <summary>
        /// A trio of 3D coordinates facing to the north.
        /// </summary>
        public static readonly Point2_Int North = new Point2_Int(0, -1);

        /// <summary>
        /// A trio of 3D coordinates facing to the south.
        /// </summary>
        public static readonly Point2_Int South = new Point2_Int(0, 1);

        #endregion

        /// <summary>
        /// Determines whether this 2D coordinates and another are equal.
        /// </summary>
        /// <param name="other">The other coordinates.</param>
        /// <returns></returns>
        public bool Equals(Point2_Int other)
        {
            return other.X.Equals(X) && other.Z.Equals(Z);
        }

        /// <summary>
        /// Determines whether this and another object are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Point2_Int)) return false;
            return Equals((Point2_Int)obj);
        }

        /// <summary>
        /// Returns the hash code for this 2D coordinates.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }
    }
}
