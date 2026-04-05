using System.Buffers;

namespace McProtoNet.Serialization;

/// <summary>
/// Represents a rented memory block from <see cref="ArrayPool{T}"/>.
/// Dispose to return memory to the pool. Avoid double-dispose.
/// </summary>
public struct MemoryOwner<T> : IMemoryOwner<T>
{
    private T[]? _array;
    private int _length;

    internal MemoryOwner(T[] array, int length)
    {
        _array = array;
        _length = length;
    }

    /// <summary>Allocates exactly <paramref name="length"/> elements from the shared pool.</summary>
    public static MemoryOwner<T> Allocate(int length)
    {
        if (length == 0) return default;
        var array = ArrayPool<T>.Shared.Rent(length);
        return new MemoryOwner<T>(array, length);
    }

    public bool IsEmpty => _array is null || _length == 0;

    public int Length => _length;

    public Memory<T> Memory => _array is null ? Memory<T>.Empty : _array.AsMemory(0, _length);

    public Span<T> Span => _array is null ? Span<T>.Empty : _array.AsSpan(0, _length);

    /// <summary>
    /// Tries to resize the view to <paramref name="newLength"/> without reallocation.
    /// Returns <c>true</c> if the underlying array is large enough; <c>false</c> otherwise.
    /// </summary>
    public bool TryResize(int newLength)
    {
        if (_array is not null && (uint)newLength <= (uint)_array.Length)
        {
            _length = newLength;
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        var arr = _array;
        if (arr is not null)
        {
            _array = null;
            ArrayPool<T>.Shared.Return(arr);
        }
    }
}
