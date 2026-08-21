using System.Buffers;

namespace McProtoNet.Transport.Framing;

internal sealed class PooledBufferWriter : IBufferWriter<byte>, IDisposable
{
    private byte[] _array;
    private int _written;

    public PooledBufferWriter(int initialCapacity)
    {
        _array = ArrayPool<byte>.Shared.Rent(Math.Max(initialCapacity, 16));
    }

    public int Length => _written;

    public Span<byte> WrittenSpan => _array.AsSpan(0, _written);

    public ReadOnlyMemory<byte> WrittenMemory => _array.AsMemory(0, _written);

    public void Clear() => _written = 0;

    public void Advance(int count) => _written += count;

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return _array.AsMemory(_written);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        Ensure(sizeHint);
        return _array.AsSpan(_written);
    }

    private void Ensure(int sizeHint)
    {
        if (sizeHint <= 0) sizeHint = 1;
        if (_array.Length - _written >= sizeHint) return;

        long wanted = Math.Max((long)_written + sizeHint, (long)_array.Length * 2);
        if (wanted > Array.MaxLength) wanted = Array.MaxLength;

        var next = ArrayPool<byte>.Shared.Rent((int)wanted);
        _array.AsSpan(0, _written).CopyTo(next);
        ArrayPool<byte>.Shared.Return(_array);
        _array = next;
    }

    public void Dispose()
    {
        var array = _array;
        _array = [];
        _written = 0;
        if (array.Length > 0) ArrayPool<byte>.Shared.Return(array);
    }

    /// <summary>
    ///     Drops the buffer without returning it to the pool — for the case where a write the
    ///     operating system may still be reading from was abandoned.
    /// </summary>
    public void Abandon()
    {
        _array = [];
        _written = 0;
    }
}
