namespace McProtoNet.Scenarios;

/// <summary>
///     The packet stream behind a <see cref="PacketWaiter" /> ended (disconnect, <see cref="PacketWaiter.Complete" />,
///     or the waiter itself was disposed). Every pending and future await on that waiter throws this;
///     the transport-side cause, when there is one, is the inner exception.
/// </summary>
public sealed class PacketStreamClosedException : Exception
{
    public PacketStreamClosedException(Exception? inner = null)
        : base(inner is null ? "The packet stream is closed." : $"The packet stream is closed: {inner.Message}", inner)
    {
    }
}
