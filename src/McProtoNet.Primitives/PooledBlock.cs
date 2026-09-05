using System.Buffers;

namespace McProtoNet.Primitives;

/// <summary>
/// Represents an array rented from an <see cref="ArrayPool{T}"/> and shared through a reference
/// count.
/// </summary>
/// <remarks>
/// A new block carries one reference, held by whoever rented it. Every further holder takes one with
/// <see cref="Retain"/> and gives it back with <see cref="Release"/>; the release that brings the
/// count to zero returns the array to the pool, after which <see cref="Array"/> is empty. All
/// members can be called from any thread.
/// </remarks>
public sealed class PooledBlock
{
    /// <summary>What <see cref="Array"/> points at once the last reference is released.</summary>
    private static readonly byte[] Released = System.Array.Empty<byte>();

    private readonly ArrayPool<byte> _pool;
    private byte[] _array;
    private int _references = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="PooledBlock"/> class over an array rented from
    /// <see cref="ArrayPool{T}.Shared"/>.
    /// </summary>
    /// <param name="minimumLength">The minimum length of the array.</param>
    public PooledBlock(int minimumLength) : this(ArrayPool<byte>.Shared, minimumLength)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PooledBlock"/> class over an array rented from
    /// the specified pool.
    /// </summary>
    /// <param name="pool">The pool to rent the array from and return it to.</param>
    /// <param name="minimumLength">The minimum length of the array.</param>
    /// <exception cref="ArgumentNullException"><paramref name="pool"/> is <see langword="null"/>.</exception>
    public PooledBlock(ArrayPool<byte> pool, int minimumLength)
    {
        ArgumentNullException.ThrowIfNull(pool);
        _pool = pool;
        _array = pool.Rent(minimumLength);
    }

    /// <summary>
    /// Gets the rented array, or an empty array once the last reference is released.
    /// </summary>
    public byte[] Array => _array;

    /// <summary>
    /// Gets the length of the rented array. This can be larger than the length that was asked for.
    /// </summary>
    public int Length => _array.Length;

    /// <summary>
    /// Gets the number of references currently held.
    /// </summary>
    public int References => Volatile.Read(ref _references);

    /// <summary>
    /// Gets a value indicating whether more than one reference is held.
    /// </summary>
    /// <remarks>
    /// A holder that sees <see langword="false"/> is the only holder and may overwrite the array. A
    /// second reference can only be taken through an existing one, so the answer holds as long as the
    /// sole holder hands out no reference while it acts on it.
    /// </remarks>
    public bool IsShared => Volatile.Read(ref _references) > 1;

    /// <summary>
    /// Takes one more reference.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The last reference was already released.</exception>
    public void Retain()
    {
        int seen;
        do
        {
            seen = Volatile.Read(ref _references);
            if (seen <= 0) ThrowReleased();
        } while (Interlocked.CompareExchange(ref _references, seen + 1, seen) != seen);

        static void ThrowReleased() => throw new ObjectDisposedException(nameof(PooledBlock));
    }

    /// <summary>
    /// Gives one reference back. The release that brings the count to zero returns the array to the
    /// pool.
    /// </summary>
    /// <exception cref="InvalidOperationException">More references were released than were held.</exception>
    public void Release()
    {
        var left = Interlocked.Decrement(ref _references);
        if (left > 0) return;
        if (left < 0) ThrowOverReleased();

        _pool.Return(Interlocked.Exchange(ref _array, Released));

        static void ThrowOverReleased() =>
            throw new InvalidOperationException("The block was released more often than it was retained");
    }
}
