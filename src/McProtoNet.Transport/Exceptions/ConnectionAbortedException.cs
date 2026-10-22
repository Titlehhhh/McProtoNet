namespace McProtoNet.Transport;

/// <summary>
/// The exception that is thrown when a member of a connection is called after the connection was
/// closed, or when a read or a write fails because the connection went down.
/// </summary>
/// <remarks>
/// <see cref="Exception.InnerException"/> holds the reason: the exception passed to the abort call, or
/// the failure the connection latched. It is <see langword="null"/> for a clean close.
/// </remarks>
public sealed class ConnectionAbortedException : IOException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionAbortedException"/> class with the
    /// specified reason and a message derived from it.
    /// </summary>
    /// <param name="reason">The exception that closed the connection, or <see langword="null"/> for a
    /// clean close. The default value is <see langword="null"/>.</param>
    public ConnectionAbortedException(Exception? reason = null)
        : base(BuildMessage(reason), reason)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionAbortedException"/> class with the
    /// specified message and reason.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="reason">The exception that closed the connection, or <see langword="null"/> for a
    /// clean close.</param>
    public ConnectionAbortedException(string message, Exception? reason)
        : base(message, reason)
    {
    }

    private static string BuildMessage(Exception? reason) =>
        reason is null
            ? "The connection is closed."
            : $"The connection was aborted: {reason.Message}";
}
