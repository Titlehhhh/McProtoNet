namespace McProtoNet.Transport;

/// <summary>
///     Thrown by a read, a flush or any member of a connection that has been aborted.
///     <see cref="Exception.InnerException" /> is the reason passed to <c>Abort</c>, or null for a
///     clean close.
/// </summary>
public sealed class ConnectionAbortedException : IOException
{
    public ConnectionAbortedException(Exception? reason = null)
        : base(reason?.Message ?? "The connection was closed.", reason)
    {
    }

    public ConnectionAbortedException(string message, Exception? reason)
        : base(message, reason)
    {
    }
}
