namespace McProtoNet.NBT;

/// <summary>
/// The exception that is thrown when an operation is attempted on an <see cref="NbtReader"/> that cannot
/// recover from a previous parsing error.
/// </summary>
public sealed class InvalidReaderStateException : InvalidOperationException
{
    internal InvalidReaderStateException(string message)
        : base(message)
    {
    }
}