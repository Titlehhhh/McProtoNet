using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Represents a quantized three-dimensional vector that the protocol uses for velocities and hit
/// offsets.
/// </summary>
/// <param name="X">The X component of the vector.</param>
/// <param name="Y">The Y component of the vector.</param>
/// <param name="Z">The Z component of the vector.</param>
/// <remarks>
/// This type exists from protocol 773 (1.21.9) onward. A zero vector occupies a single byte.
/// Any other value occupies 48 bits that hold the two low bits of the scale, a continuation flag
/// and three 15-bit components; when the continuation flag is set, the remaining bits of the scale
/// follow as a VarInt. Components are clamped and rounded on write, so a value read back is not
/// exactly the value written.
/// </remarks>
[ProtocolSupport(MinecraftVersion.V1_21_9_To_1_21_10_Protocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct LpVec3(double X, double Y, double Z) : IProtocolType<LpVec3>
{
    private const double MaxQuantized = 32766.0;
    private const double AbsMax = 1.7179869183E10;
    private const double AbsMin = 3.051944088384301E-5;

    /// <summary>
    /// Reads an <see cref="LpVec3"/> from the specified reader.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <returns>The vector that was read.</returns>
    /// <exception cref="ProtocolNotSupportException"><paramref name="protocolVersion"/> does not
    /// support this type.</exception>
    public static LpVec3 Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LpVec3>(protocolVersion);

        uint first = reader.ReadUnsignedByte();
        if (first == 0) return new LpVec3(0d, 0d, 0d);

        uint second = reader.ReadUnsignedByte();
        ulong packed = ((ulong)reader.ReadUnsignedInt() << 16) | ((ulong)second << 8) | first;

        long scale = (long)(first & 3);
        if ((first & 4) != 0) scale |= (long)(uint)reader.ReadVarInt() << 2;

        return new LpVec3(
            Unpack(packed >> 3) * scale,
            Unpack(packed >> 18) * scale,
            Unpack(packed >> 33) * scale);
    }

    /// <summary>
    /// Writes the current vector to the specified writer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="protocolVersion">The protocol version of the connection.</param>
    /// <exception cref="ProtocolNotSupportException"><paramref name="protocolVersion"/> does not
    /// support this type.</exception>
    /// <remarks>
    /// A component that is <see cref="double.NaN"/> is written as zero. Other components are clamped
    /// to the representable range. A vector whose largest component is below the smallest
    /// representable magnitude is written as the zero vector.
    /// </remarks>
    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LpVec3>(protocolVersion);

        double x = Clamp(X), y = Clamp(Y), z = Clamp(Z);
        double max = Math.Max(Math.Abs(x), Math.Max(Math.Abs(y), Math.Abs(z)));

        if (max < AbsMin)
        {
            writer.WriteUnsignedByte(0);
            return;
        }

        long scale = (long)Math.Ceiling(max);
        double inv = 1d / scale;
        bool continued = (scale & 3) != scale;

        ulong packed =
            (ulong)(scale & 3)
            | (continued ? 4UL : 0UL)
            | (Pack(x * inv) << 3)
            | (Pack(y * inv) << 18)
            | (Pack(z * inv) << 33);

        writer.WriteUnsignedByte((byte)(packed & 0xFF));
        writer.WriteUnsignedByte((byte)((packed >> 8) & 0xFF));
        writer.WriteUnsignedInt((uint)(packed >> 16));

        if (continued) writer.WriteVarInt((int)(scale >> 2));
    }

    private static double Unpack(ulong packed) =>
        Math.Min(packed & 0x7FFF, MaxQuantized) * 2d / MaxQuantized - 1d;

    // Java rounds half up here; Math.Round would bank on .5 and shift the packed byte.
    private static ulong Pack(double value) =>
        (ulong)(long)Math.Floor((value * 0.5d + 0.5d) * MaxQuantized + 0.5d);

    private static double Clamp(double value) =>
        double.IsNaN(value) ? 0d : Math.Clamp(value, -AbsMax, AbsMax);
}
