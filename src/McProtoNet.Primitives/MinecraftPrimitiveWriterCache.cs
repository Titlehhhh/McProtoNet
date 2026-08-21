namespace McProtoNet.Primitives;

/// <summary>
/// Thread-local cache for <see cref="MinecraftPrimitiveWriter"/> instances, in the shape
/// <c>Utf8JsonWriter</c> uses: one writer per thread, reset on rent instead of reallocated,
/// so a send path does not allocate a writer and its buffer per packet.
/// <para>
/// Contract: rent and return in a <c>try/finally</c>, return exactly once, and touch nothing on
/// the writer after returning it — the next renter resets it and writes over the same buffer.
/// A writer whose buffer grew past the discard cap is dropped rather than cached, so one
/// oversized packet does not pin a large array to a thread.
/// </para>
/// <para>
/// The dangerous shape is reading, not writing: <see cref="MinecraftPrimitiveWriter.WrittenMemory" />
/// and <see cref="MinecraftPrimitiveWriter.WrittenSpan" /> are windows into the writer's own buffer,
/// so a caller who returns the writer and then hands that window to someone else is passing memory
/// the next renter is free to overwrite — silently, with no exception. Return last, after the bytes
/// have been consumed. In DEBUG, returning the same writer twice in a row on one thread throws.
/// </para>
/// <para>
/// Async callers may resume on a different thread than they rented on; the writer then lands in
/// that thread's slot. Safe (the slot is cleared on rent, so no two holders share a writer),
/// only slightly less effective.
/// </para>
/// </summary>
public static class MinecraftPrimitiveWriterCache
{
    // Writers larger than this are discarded to avoid holding large arrays in cache
    private const int MaxCachedCapacity = 64 * 1024;

    [ThreadStatic]
    private static MinecraftPrimitiveWriter? _cached;

    /// <summary>Rents a reset writer from the cache, or allocates a new one.</summary>
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

    /// <summary>Returns a writer to the cache. Discards it if its internal buffer is too large.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="writer" /> is <c>null</c>.</exception>
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
