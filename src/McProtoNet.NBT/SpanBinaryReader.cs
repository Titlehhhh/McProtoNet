using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace McProtoNet.NBT;

[StructLayout(LayoutKind.Auto)]
internal ref struct SpanBinaryReader
{
    private readonly ReadOnlySpan<byte> _span;
    private int _position;

    public int ConsumedCount => _position;
    public int RemainingCount => _span.Length - _position;

    internal SpanBinaryReader(ReadOnlySpan<byte> span)
    {
        _span = span;
        _position = 0;
    }

    internal byte Read()
    {
        if ((uint)_position >= (uint)_span.Length) ThrowEOF();
        return _span[_position++];
    }

    internal ReadOnlySpan<byte> Read(int count)
    {
        int end = _position + count;
        if ((uint)end > (uint)_span.Length) ThrowEOF();
        var result = _span.Slice(_position, count);
        _position = end;
        return result;
    }

    internal short ReadBigEndian16() => BinaryPrimitives.ReadInt16BigEndian(Read(sizeof(short)));
    internal int ReadBigEndian32() => BinaryPrimitives.ReadInt32BigEndian(Read(sizeof(int)));
    internal long ReadBigEndian64() => BinaryPrimitives.ReadInt64BigEndian(Read(sizeof(long)));

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowEOF() => throw new EndOfStreamException("Unexpected end of NBT data.");
}
