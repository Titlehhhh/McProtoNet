namespace McProtoNet.NBT;

/// <summary>
///     Exception thrown when a format violation is detected while
///     parsing or serializing an NBT file.
/// </summary>
public sealed class NbtFormatException : Exception
{
    public NbtFormatException(string message)
        : base(message)
    {
    }
}