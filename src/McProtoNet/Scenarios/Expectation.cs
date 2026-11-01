using System.Runtime.ExceptionServices;
using System.Threading.Channels;
using McProtoNet.Protocol;

namespace McProtoNet.Scenarios;

/// <summary>
///     One open wait for packets of type T: a bounded queue that fills from the receive loop for as
///     long as the handle lives. Open it <b>before</b> sending the trigger — the queue is live when
///     <see cref="PacketWaiter.Expect{T}" /> returns, so nothing that arrives after can be missed.
///     Single consumer: one handle belongs to one scenario flow.
/// </summary>
public sealed class Expectation<T> : IExpectation, IDisposable where T : class, IPacket<T>
{
    private readonly Channel<T> _queue;
    private readonly Func<T, bool>? _filter;
    private readonly int _capacity;
    private readonly OverflowPolicy _overflow;
    private readonly PacketWaiter _owner;
    private int _disposed;

    internal Expectation(PacketWaiter owner, Func<T, bool>? filter, int capacity, OverflowPolicy overflow)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        _owner = owner;
        _filter = filter;
        _capacity = capacity;
        _overflow = overflow;
        _queue = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = false,
            FullMode = overflow switch
            {
                OverflowPolicy.DropOldest => BoundedChannelFullMode.DropOldest,
                OverflowPolicy.DropNewest => BoundedChannelFullMode.DropNewest,
                _ => BoundedChannelFullMode.Wait
            }
        });
    }

    /// <summary>Packets queued and not yet taken.</summary>
    public int Count => _queue.Reader.Count;

    /// <summary>Receive-loop side: filter, then enqueue. Never blocks, never throws into the loop.</summary>
    internal void Offer(T packet)
    {
        if (_filter is not null)
        {
            bool pass;
            try
            {
                pass = _filter(packet);
            }
            catch (Exception e)
            {
                Fault(e);
                return;
            }

            if (!pass)
                return;
        }

        if (!_queue.Writer.TryWrite(packet) && _overflow == OverflowPolicy.Fault)
            Fault(new PacketOverflowException(typeof(T), _capacity));
    }

    internal void Fault(Exception error) => _queue.Writer.TryComplete(error);

    /// <summary>Next packet that passed the filter; waits as long as the token allows.</summary>
    public async ValueTask<T> NextAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _queue.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (ChannelClosedException e)
        {
            throw Closed(e);
        }
    }

    /// <summary>Next packet that passed the filter, or <see cref="TimeoutException" /> after <paramref name="timeout" />.</summary>
    public async ValueTask<T> NextAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linked.CancelAfter(timeout);
        try
        {
            return await _queue.Reader.ReadAsync(linked.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"{typeof(T).Name} did not arrive within {timeout}.");
        }
        catch (ChannelClosedException e)
        {
            throw Closed(e);
        }
    }

    /// <summary>
    ///     Next packet for which <paramref name="match" /> is true; the ones it rejects are consumed and
    ///     dropped. For cascades where the condition is known only after the wait was opened.
    /// </summary>
    public async ValueTask<T> FirstAsync(Func<T, bool> match, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(match);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linked.CancelAfter(timeout);
        try
        {
            while (true)
            {
                var packet = await _queue.Reader.ReadAsync(linked.Token).ConfigureAwait(false);
                if (match(packet))
                    return packet;
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"No matching {typeof(T).Name} arrived within {timeout}.");
        }
        catch (ChannelClosedException e)
        {
            throw Closed(e);
        }
    }

    /// <summary>Takes a queued packet without waiting.</summary>
    public bool TryTake(out T packet) => _queue.Reader.TryRead(out packet!);

    ValueTask<bool> IExpectation.WaitToReadAsync(CancellationToken cancellationToken)
        => _queue.Reader.WaitToReadAsync(cancellationToken);

    bool IExpectation.TryTake(out object packet)
    {
        if (_queue.Reader.TryRead(out var typed))
        {
            packet = typed;
            return true;
        }

        packet = null!;
        return false;
    }

    void IExpectation.ThrowIfClosed()
    {
        var completion = _queue.Reader.Completion;
        if (!completion.IsCompleted)
            return;
        if (completion.Exception?.InnerException is { } error)
            ExceptionDispatchInfo.Throw(error);
        throw new PacketStreamClosedException();
    }

    /// <summary>Unsubscribes and releases the queue. A pending await throws <see cref="ObjectDisposedException" />.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;
        _owner.Remove(this);
        _queue.Writer.TryComplete(new ObjectDisposedException(nameof(Expectation<T>)));
    }

    private static Exception Closed(ChannelClosedException e)
    {
        if (e.InnerException is { } cause)
            ExceptionDispatchInfo.Throw(cause);
        return new PacketStreamClosedException();
    }
}
