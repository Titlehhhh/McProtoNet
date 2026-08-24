namespace McProtoNet.NBT;

/// <summary>
/// The exception that is thrown when a format violation is detected while NBT data is parsed or serialized.
/// </summary>
public sealed class NbtFormatException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NbtFormatException"/> class with the specified error
    /// message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NbtFormatException(string message)
        : base(message)
    {
    }
}