namespace McProtoNet.Scenarios;

/// <summary>
///     Untyped view of an expectation, enough for <see cref="PacketWait.AnyAsync" /> to race several of
///     them without taking a packet from the ones that lose.
/// </summary>
public interface IExpectation
{
    /// <summary>Completes when a packet can be taken, or with false when the expectation is closed.</summary>
    ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken);

    /// <summary>Takes one queued packet without waiting.</summary>
    bool TryTake(out object packet);

    /// <summary>Throws the closing error if the expectation is closed; otherwise does nothing.</summary>
    void ThrowIfClosed();
}
