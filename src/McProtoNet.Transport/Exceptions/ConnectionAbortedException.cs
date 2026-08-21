namespace McProtoNet.Transport;

/// <summary>
///     Thrown by a read, a write or any member of a connection that is closed. It says the connection
///     went down, and <see cref="Exception.InnerException" /> — the reason passed to <c>Abort</c> or
///     the failure the connection latched — says why. Null inner means a clean close.
/// </summary>
public sealed class ConnectionAbortedException : IOException
{
    public ConnectionAbortedException(Exception? reason = null)
        : base(BuildMessage(reason), reason)
    {
    }

    public ConnectionAbortedException(string message, Exception? reason)
        : base(message, reason)
    {
    }

    private static string BuildMessage(Exception? reason) =>
        reason is null
            ? "The connection is closed."
            : $"The connection was aborted: {reason.Message}";
}
