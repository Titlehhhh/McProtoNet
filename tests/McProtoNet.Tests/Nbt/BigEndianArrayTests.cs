using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using McProtoNet.NBT;

namespace McProtoNet.Tests.Nbt;

/// <summary>
/// Endianness helper shared by every NBT reader and writer: big-endian bytes to
/// unmanaged elements and back, for the exact set of element types NBT uses.
/// </summary>
public class BigEndianArrayTests
{
    private struct Unsupported
    {
        public int A;
        public int B;
        public int C;
    }

    private static byte[] BigEndianBytes<T>(T[] values) where T : unmanaged
    {
        var bytes = MemoryMarshal.AsBytes(values.AsSpan()).ToArray();
        if (!BitConverter.IsLittleEndian) return bytes;
        var size = Unsafe.SizeOf<T>();
        for (var offset = 0; offset < bytes.Length; offset += size)
            bytes.AsSpan(offset, size).Reverse();
        return bytes;
    }

    private static void RoundTrip<T>(T[] values) where T : unmanaged
    {
        var wire = BigEndianBytes(values);

        var decoded = new T[values.Length];
        BigEndianArray.FromBigEndian<T>(wire, decoded);
        Assert.Equal(values, decoded);

        var encoded = new byte[wire.Length];
        BigEndianArray.ToBigEndian<T>(decoded, encoded);
        Assert.Equal(wire, encoded);
    }

    [Fact]
    public void RoundTripsShort() => RoundTrip<short>([short.MinValue, -1, 0, 1, 0x0102, short.MaxValue]);

    [Fact]
    public void RoundTripsUShort() => RoundTrip<ushort>([ushort.MinValue, 1, 0x0102, ushort.MaxValue]);

    [Fact]
    public void RoundTripsInt() => RoundTrip<int>([int.MinValue, -1, 0, 1, 0x01020304, int.MaxValue]);

    [Fact]
    public void RoundTripsUInt() => RoundTrip<uint>([uint.MinValue, 1, 0x01020304, uint.MaxValue]);

    [Fact]
    public void RoundTripsLong() => RoundTrip<long>([long.MinValue, -1, 0, 1, 0x0102030405060708, long.MaxValue]);

    [Fact]
    public void RoundTripsULong() => RoundTrip<ulong>([ulong.MinValue, 1, 0x0102030405060708, ulong.MaxValue]);

    [Fact]
    public void RoundTripsFloat() => RoundTrip<float>([float.MinValue, -0.0f, 0f, 3.14f, float.MaxValue]);

    [Fact]
    public void RoundTripsDouble() => RoundTrip<double>([double.MinValue, -0.0, 0, 2.71828, double.MaxValue]);

    [Fact]
    public void RoundTripsByte() => RoundTrip<byte>([0, 1, 127, 128, 255]);

    [Fact]
    public void RoundTripsSByte() => RoundTrip<sbyte>([sbyte.MinValue, -1, 0, 1, sbyte.MaxValue]);

    [Fact]
    public void FromBigEndianMatchesBinaryPrimitivesForInt()
    {
        byte[] wire = [0x00, 0x00, 0x00, 0x01, 0x7F, 0xFF, 0xFF, 0xFF];
        var decoded = new int[2];
        BigEndianArray.FromBigEndian<int>(wire, decoded);
        Assert.Equal(BinaryPrimitives.ReadInt32BigEndian(wire), decoded[0]);
        Assert.Equal(BinaryPrimitives.ReadInt32BigEndian(wire.AsSpan(4)), decoded[1]);
    }

    [Fact]
    public void FromBigEndianWorksInPlace()
    {
        int[] values = [0x01020304, unchecked((int)0xF0E0D0C0)];
        var wire = BigEndianBytes(values);
        var buffer = new int[values.Length];
        wire.CopyTo(MemoryMarshal.AsBytes(buffer.AsSpan()));
        BigEndianArray.FromBigEndian<int>(MemoryMarshal.AsBytes(buffer.AsSpan()), buffer);
        Assert.Equal(values, buffer);
    }

    [Fact]
    public void EmptyIsAllowed()
    {
        BigEndianArray.FromBigEndian<int>([], []);
        BigEndianArray.ToBigEndian<int>([], []);
    }

    [Fact]
    public void FromBigEndianRejectsUnsupportedElement()
    {
        var destination = new Unsupported[1];
        var source = new byte[Unsafe.SizeOf<Unsupported>()];
        Assert.Throws<NotSupportedException>(() => BigEndianArray.FromBigEndian<Unsupported>(source, destination));
    }

    [Fact]
    public void ToBigEndianRejectsUnsupportedElement()
    {
        var source = new Unsupported[1];
        var destination = new byte[Unsafe.SizeOf<Unsupported>()];
        Assert.Throws<NotSupportedException>(() => BigEndianArray.ToBigEndian<Unsupported>(source, destination));
    }

    [Fact]
    public void MismatchedLengthsAreRejected()
    {
        Assert.Throws<ArgumentException>(() => BigEndianArray.FromBigEndian<int>(new byte[6], new int[2]));
        Assert.Throws<ArgumentException>(() => BigEndianArray.ToBigEndian<int>(new int[2], new byte[6]));
    }
}
