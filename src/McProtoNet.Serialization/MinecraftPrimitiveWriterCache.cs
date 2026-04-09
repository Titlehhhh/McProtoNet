namespace McProtoNet.Serialization;

/// <summary>
/// Thread-local cache for <see cref="MinecraftPrimitiveWriter"/> instances.
/// </summary>
internal static class MinecraftPrimitiveWriterCache
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
    public static void Return(MinecraftPrimitiveWriter writer)
    {
        if (writer.Capacity <= MaxCachedCapacity)
            _cached = writer;
    }
}
