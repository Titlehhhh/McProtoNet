using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Quantized vector the protocol uses for velocities and hit offsets from 1.21.9 on. One byte
/// carries the zero vector; otherwise 48 bits hold a two-bit scale, a continuation flag and three
/// 15-bit components, and the rest of the scale follows as a VarInt.
/// </summary>
[ProtocolSupport(MinecraftVersion.V1_21_9_To_1_21_10_Protocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct LpVec3(double X, double Y, double Z) : IProtocolType<LpVec3>
{
    private const double MaxQuantized = 32766.0;
    private const double AbsMax = 1.7179869183E10;
    private const double AbsMin = 3.051944088384301E-5;

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
