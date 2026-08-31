using System.Buffers;

namespace McProtoNet.Primitives;

/// <summary>
/// Represents a block of memory rented from <see cref="ArrayPool{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the elements in the memory block.</typeparam>
/// <remarks>
/// Disposing the instance returns the array to the pool. This is a mutable structure, so a copy must be
/// made only to transfer ownership; a copy that outlives the transfer still holds the array and returns
/// it a second time, which corrupts the pool. Disposing one instance twice has no effect the second
/// time.
/// </remarks>
public struct MemoryOwner<T> : IMemoryOwner<T>
{
    private T[]? _array;
    private int _length;

    internal MemoryOwner(T[] array, int length)
    {
        _array = array;
        _length = length;
    }

    /// <summary>
    /// Rents a block of at least the specified number of elements from the shared pool and returns a view
    /// of exactly that length.
    /// </summary>
    /// <param name="length">The number of elements to make visible. This value must not be
    /// negative.</param>
    /// <returns>A new <see cref="MemoryOwner{T}"/> over the rented array, or an empty instance if
    /// <paramref name="length"/> is 0.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than zero.</exception>
    public static MemoryOwner<T> Allocate(int length)
    {
        if (length == 0) return default;
        var array = ArrayPool<T>.Shared.Rent(length);
        return new MemoryOwner<T>(array, length);
    }

    /// <summary>
    /// Gets a value indicating whether the instance holds no elements.
    /// </summary>
    /// <value><see langword="true"/> if no array is held or its visible length is 0; otherwise,
    /// <see langword="false"/>.</value>
    public bool IsEmpty => _array is null || _length == 0;

    /// <summary>
    /// Gets the number of elements the instance makes visible.
    /// </summary>
    public int Length => _length;

    /// <summary>
    /// Gets the rented block as memory.
    /// </summary>
    public Memory<T> Memory => _array is null ? Memory<T>.Empty : _array.AsMemory(0, _length);

    /// <summary>
    /// Gets the rented block as a span.
    /// </summary>
    public Span<T> Span => _array is null ? Span<T>.Empty : _array.AsSpan(0, _length);

    /// <summary>
    /// Attempts to change the visible length without renting a new array.
    /// </summary>
    /// <param name="newLength">The new visible length. This value must not be negative.</param>
    /// <returns><see langword="true"/> if the rented array is large enough and the length was changed;
    /// otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// Elements between the old and the new length keep whatever the pool left in them.
    /// </remarks>
    public bool TryResize(int newLength)
    {
        if (_array is not null && (uint)newLength <= (uint)_array.Length)
        {
            _length = newLength;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="MemoryOwner{T}"/> structure by
    /// returning its array to the pool.
    /// </summary>
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
