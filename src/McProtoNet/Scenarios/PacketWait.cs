using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace McProtoNet.Scenarios;

/// <summary>Composition helpers around <see cref="PacketWaiter" />.</summary>
public static class PacketWait
{
    /// <summary>Hands every dispatched packet to both visitors; one decode, two consumers.</summary>
    public static TeeVisitor<TA, TB> Tee<TA, TB>(TA a, TB b)
        where TA : IPacketVisitor where TB : IPacketVisitor => new(a, b);

    /// <summary>
    ///     First packet from any of <paramref name="expectations" />. The losers keep what they
    ///     received: a cancelled wait never takes a packet out of a queue.
    /// </summary>
    public static async Task<object> AnyAsync(TimeSpan timeout, CancellationToken cancellationToken,
        params IExpectation[] expectations)
    {
        ArgumentNullException.ThrowIfNull(expectations);
        if (expectations.Length == 0)
            throw new ArgumentException("At least one expectation is required.", nameof(expectations));

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linked.CancelAfter(timeout);
        try
        {
            while (true)
            {
                foreach (var expectation in expectations)
                {
                    if (expectation.TryTake(out var packet))
                        return packet;
                    expectation.ThrowIfClosed();
                }

                var waits = new Task<bool>[expectations.Length];
                for (var i = 0; i < expectations.Length; i++)
                    waits[i] = expectations[i].WaitToReadAsync(linked.Token).AsTask();
                await Task.WhenAny(waits).ConfigureAwait(false);
                linked.Token.ThrowIfCancellationRequested();
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"None of the {expectations.Length} expected packets arrived within {timeout}.");
        }
        finally
        {
            linked.Cancel();
        }
    }
}
