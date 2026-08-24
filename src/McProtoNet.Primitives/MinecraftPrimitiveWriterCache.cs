namespace McProtoNet.Primitives;

/// <summary>
/// Provides a thread-local cache of <see cref="MinecraftPrimitiveWriter"/> instances, so a send path
/// does not allocate a writer and its buffer for every packet.
/// </summary>
/// <remarks>
/// <para>
/// One writer is held per thread and is reset when it is rented instead of being reallocated. Each
/// rented writer must be returned exactly once, and no member of it may be used after it is returned;
/// the next renter resets it and writes over the same buffer.
/// </para>
/// <para>
/// <see cref="MinecraftPrimitiveWriter.WrittenMemory"/> and
/// <see cref="MinecraftPrimitiveWriter.WrittenSpan"/> are windows into the writer's own buffer. A window
/// passed on after the writer is returned refers to memory the next renter can overwrite, and no
/// exception is raised. The writer is returned last, after the bytes have been consumed.
/// </para>
/// <para>
/// A writer whose buffer grew past the cap of 64 kilobytes is discarded instead of cached.
/// </para>
/// <para>
/// An async caller can resume on a thread other than the one it rented on, in which case the writer
/// lands in that thread's slot. The slot is cleared when a writer is rented, so no two holders share a
/// writer.
/// </para>
/// <para>
/// In debug builds, returning the same writer twice in a row on one thread throws
/// <see cref="InvalidOperationException"/>.
/// </para>
/// </remarks>
public static class MinecraftPrimitiveWriterCache
{
    // Writers larger than this are discarded to avoid holding large arrays in cache
    private const int MaxCachedCapacity = 64 * 1024;

    [ThreadStatic]
    private static MinecraftPrimitiveWriter? _cached;

    /// <summary>
    /// Rents a writer from the current thread's slot, or creates one if the slot is empty.
    /// </summary>
    /// <returns>A writer with no written bytes.</returns>
    public static MinecraftPrimitiveWriter Rent()
    {
        var writer = _cached;
        if (writer is not null)
        {
            _cached = null;
            writer.Reset();
            return writer;
        }

        return new MinecraftPrimitiveWriter();
    }

    /// <summary>
    /// Returns a writer to the current thread's slot, or discards it if its buffer grew past the cap.
    /// </summary>
    /// <param name="writer">The writer to return. It must not be used after this call.</param>
    /// <exception cref="ArgumentNullException"><paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">In debug builds only: <paramref name="writer"/> is
    /// already in this thread's slot.</exception>
    public static void Return(MinecraftPrimitiveWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);

#if DEBUG
        // A writer returned twice is either a double `finally` or a writer that was never rented
        // here; both end with two holders writing over one buffer. Only the same-thread repeat is
        // catchable this cheaply, which is the shape a wrong `finally` produces.
        if (ReferenceEquals(_cached, writer))
            throw new InvalidOperationException(
                "This MinecraftPrimitiveWriter is already in the cache: it was returned twice, " +
                "or returned by a thread that did not rent it.");
#endif

        if (writer.Capacity <= MaxCachedCapacity)
            _cached = writer;
    }
}
