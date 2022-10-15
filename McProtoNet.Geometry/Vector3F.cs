using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Geometry
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Vector3F : IEquatable<Vector3F>
    {
        /// <summary>
        /// The X component of this vector.
        /// </summary>
        [FieldOffset(0)]
        public float X;

        /// <summary>
        /// The Y component of this vector.
        /// </summary>
        [FieldOffset(8)]
        public float Y;

        /// <summary>
        /// The Z component of this vector.
        /// </summary>
        [FieldOffset(16)]
        public float Z;

        /// <summary>
        /// Creates a new vector from the specified value.
        /// </summary>
        /// <param name="value">The value for the components of the vector.</param>
        public Vector3F(float value)
        {
            X = Y = Z = value;
        }
        public Vector3F(Point3F location1, Point3F location2)
        {
            X = location2.X - location1.X;
            Y = location2.Y - location1.Y;
            Z = location2.Z - location1.Z;
        }

        /// <summary>
        /// Creates a new vector from the specified values.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        /// <param name="z">The Z component of the vector.</param>
        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a new vector from copying another.
        /// </summary>
        /// <param name="v">The vector to copy.</param>
        public Vector3F(Vector3F v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Converts this Vector3F to a string in the format &lt;x,y,z&gt;.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", X, Y, Z);
        }

        #region MathF

        /// <summary>
        /// Truncates the decimal component of each part of this Vector3F.
        /// </summary>
        public Vector3F Floor()
        {
            return new Vector3F(MathF.Floor(X), MathF.Floor(Y), MathF.Floor(Z));
        }

        /// <summary>
        /// Rounds the decimal component of each part of this Vector3F.
        /// </summary>
        public Vector3F Round()
        {
            return new Vector3F(MathF.Round(X), MathF.Round(Y), MathF.Round(Z));
        }

        /// <summary>
        /// Clamps the vector to within the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        public void Clamp(float value)
        {
            if (MathF.Abs(X) > value)
                X = value * (X < 0 ? -1 : 1);
            if (MathF.Abs(Y) > value)
                Y = value * (Y < 0 ? -1 : 1);
            if (MathF.Abs(Z) > value)
                Z = value * (Z < 0 ? -1 : 1);
        }

        /// <summary>
        /// Calculates the distance between two Vector3F objects.
        /// </summary>
        public float DistanceTo(Vector3F other)
        {
            return MathF.Sqrt(Square(other.X - X) +
                             Square(other.Y - Y) +
                             Square(other.Z - Z));
        }


        /// <summary>
        /// Calculates the square of a num.
        /// </summary>
        private float Square(float num)
        {
            return num * num;
        }
        public const float kEpsilon = 0.00001f;
        public Vector3F Normalize()
        {
            if (Magnitude > kEpsilon)
                return this / Magnitude;
            else
                return Vector3F.Zero;
        }
        public float Magnitude => (float)MathF.Sqrt(X * X + Y * Y + Z * Z);

        /// <summary>
        /// Finds the distance of this vector from Vector3F.Zero
        /// </summary>
        public float Distance
        {
            get
            {
                return DistanceTo(Zero);
            }
        }

        /// <summary>
        /// Returns the component-wise minumum of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns></returns>
        public static Vector3F Min(Vector3F value1, Vector3F value2)
        {
            return new Vector3F(
                MathF.Min(value1.X, value2.X),
                MathF.Min(value1.Y, value2.Y),
                MathF.Min(value1.Z, value2.Z)
                );
        }

        /// <summary>
        /// Returns the component-wise maximum of two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns></returns>
        public static Vector3F Max(Vector3F value1, Vector3F value2)
        {
            return new Vector3F(
                MathF.Max(value1.X, value2.X),
                MathF.Max(value1.Y, value2.Y),
                MathF.Max(value1.Z, value2.Z)
                );
        }

        /// <summary>
        /// Calculates the dot product between two vectors.
        /// </summary>
        public static float Dot(Vector3F value1, Vector3F value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
        }

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product of two vectors.</returns>
        public static Vector3F Cross(Vector3F vector1, Vector3F vector2)
        {
            Cross(ref vector1, ref vector2, out vector1);
            return vector1;
        }

        /// <summary>
        /// Computes the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="result">The cross product of two vectors as an output parameter.</param>
        public static void Cross(ref Vector3F vector1, ref Vector3F vector2, out Vector3F result)
        {
            var x = vector1.Y * vector2.Z - vector2.Y * vector1.Z;
            var y = -(vector1.X * vector2.Z - vector2.X * vector1.Z);
            var z = vector1.X * vector2.Y - vector2.X * vector1.Y;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        #endregion

        #region Operators

        public static bool operator !=(Vector3F a, Vector3F b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Vector3F a, Vector3F b)
        {
            return a.Equals(b);
        }

        public static Vector3F operator +(Vector3F a, Vector3F b)
        {
            return new Vector3F(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z);
        }

        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z);
        }



        public static Vector3F operator -(Vector3F a)
        {
            return new Vector3F(
                -a.X,
                -a.Y,
                -a.Z);
        }

        public static Vector3F operator *(Vector3F a, Vector3F b)
        {
            return new Vector3F(
                a.X * b.X,
                a.Y * b.Y,
                a.Z * b.Z);
        }

        public static Vector3F operator /(Vector3F a, Vector3F b)
        {
            return new Vector3F(
                a.X / b.X,
                a.Y / b.Y,
                a.Z / b.Z);
        }

        public static Vector3F operator %(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
        }

        public static Vector3F operator +(Vector3F a, float b)
        {
            return new Vector3F(
                a.X + b,
                a.Y + b,
                a.Z + b);
        }

        public static Vector3F operator -(Vector3F a, float b)
        {
            return new Vector3F(
                a.X - b,
                a.Y - b,
                a.Z - b);
        }

        public static Vector3F operator *(Vector3F a, float b)
        {
            return new Vector3F(
                a.X * b,
                a.Y * b,
                a.Z * b);
        }

        public static Vector3F operator /(Vector3F a, float b)
        {
            return new Vector3F(
                a.X / b,
                a.Y / b,
                a.Z / b);
        }

        public static Vector3F operator %(Vector3F a, float b)
        {
            return new Vector3F(a.X % b, a.Y % b, a.Y % b);
        }

        public static Vector3F operator +(float a, Vector3F b)
        {
            return new Vector3F(
                a + b.X,
                a + b.Y,
                a + b.Z);
        }

        public static Vector3F operator -(float a, Vector3F b)
        {
            return new Vector3F(
                a - b.X,
                a - b.Y,
                a - b.Z);
        }

        public static Vector3F operator *(float a, Vector3F b)
        {
            return new Vector3F(
                a * b.X,
                a * b.Y,
                a * b.Z);
        }

        public static Vector3F operator /(float a, Vector3F b)
        {
            return new Vector3F(
                a / b.X,
                a / b.Y,
                a / b.Z);
        }

        public static Vector3F operator %(float a, Vector3F b)
        {
            return new Vector3F(a % b.X, a % b.Y, a % b.Y);
        }

        #endregion

        #region Conversion operators

        public static implicit operator Vector3F(Point3_Int a)
        {
            return new Vector3F(a.X, a.Y, a.Z);
        }

        public static explicit operator Vector3F(Point2_Int c)
        {
            return new Vector3F(c.X, 0, c.Z);
        }

        public static explicit operator Vector3F(Vector3 c)
        {
            return new Vector3F((float)c.X, (float)c.Y, (float)c.Z);
        }

        #endregion

        #region Constants

        /// <summary>
        /// A vector with its components set to 0.0.
        /// </summary>
        public static readonly Vector3F Zero = new Vector3F(0);

        /// <summary>
        /// A vector with its components set to 1.0.
        /// </summary>
        public static readonly Vector3F One = new Vector3F(1);


        /// <summary>
        /// A vector that points upward.
        /// </summary>
        public static readonly Vector3F Up = new Vector3F(0, 1, 0);

        /// <summary>
        /// A vector that points downward.
        /// </summary>
        public static readonly Vector3F Down = new Vector3F(0, -1, 0);

        /// <summary>
        /// A vector that points to the left.
        /// </summary>
        public static readonly Vector3F Left = new Vector3F(-1, 0, 0);

        /// <summary>
        /// A vector that points to the right.
        /// </summary>
        public static readonly Vector3F Right = new Vector3F(1, 0, 0);

        /// <summary>
        /// A vector that points backward.
        /// </summary>
        public static readonly Vector3F Backwards = new Vector3F(0, 0, -1);

        /// <summary>
        /// A vector that points forward.
        /// </summary>
        public static readonly Vector3F Forwards = new Vector3F(0, 0, 1);


        /// <summary>
        /// A vector that points to the east.
        /// </summary>
        public static readonly Vector3F East = new Vector3F(1, 0, 0);

        /// <summary>
        /// A vector that points to the west.
        /// </summary>
        public static readonly Vector3F West = new Vector3F(-1, 0, 0);

        /// <summary>
        /// A vector that points to the north.
        /// </summary>
        public static readonly Vector3F North = new Vector3F(0, 0, -1);

        /// <summary>
        /// A vector that points to the south.
        /// </summary>
        public static readonly Vector3F South = new Vector3F(0, 0, 1);

        #endregion

        /// <summary>
        /// Determines whether this and another vector are equal.
        /// </summary>
        /// <param name="other">The other vector.</param>
        /// <returns></returns>
        public bool Equals(Vector3F other)
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
            return obj is Vector3F && Equals((Vector3F)obj);
        }

        /// <summary>
        /// Gets the hash code for this vector.
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
