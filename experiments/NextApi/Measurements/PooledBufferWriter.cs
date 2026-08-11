// EXPERIMENT (McProtoNet.Next) — measurement support. A minimal pooled
// IBufferWriter<byte>: the shape the proposed client.Output would have, used here
// only as the "staging buffer" arm of the send measurement.

using System.Buffers;

namespace McProtoNet.Next.Measurements;

/// <summary>Grow-on-demand <see cref="IBufferWriter{T}" /> over a pooled array.</summary>
internal sealed class PooledBufferWriter : IBufferWriter<byte>, IDisposable
{
    private byte[] _buffer;
    private int _written;

    public PooledBufferWriter(int initialCapacity = 256)
    {
        _buffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
    }

    public ReadOnlySpan<byte> WrittenSpan => _buffer.AsSpan(0, _written);

    public void Reset() => _written = 0;

    public void Advance(int count) => _written += count;

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return _buffer.AsMemory(_written);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return _buffer.AsSpan(_written);
    }

    public void Dispose()
    {
        byte[]? buffer = _buffer;
        _buffer = null!;
        if (buffer is not null)
            ArrayPool<byte>.Shared.Return(buffer);
    }

    private void Ensure(int sizeHint)
    {
        if (sizeHint <= 0)
            sizeHint = 1;
        if (_buffer.Length - _written >= sizeHint)
            return;

        int needed = _written + sizeHint;
        byte[] bigger = ArrayPool<byte>.Shared.Rent(Math.Max(needed, _buffer.Length * 2));
        _buffer.AsSpan(0, _written).CopyTo(bigger);
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = bigger;
    }
}
