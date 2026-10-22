using McProtoNet.Primitives;
using McProtoNet.Protocol;

namespace McProtoNet.Scenarios;

/// <summary>
///     Turns the dispatched packet stream into awaitable expectations. A plain
///     <see cref="IPacketVisitor" />: feed it from the receive loop (directly via
///     <see cref="PacketFlow.Dispatch{TVisitor}" /> or through <see cref="Dispatch" />), open
///     <see cref="Expect{T}" /> handles from scenario code, and call <see cref="Complete" /> when the
///     stream ends so no await hangs.
///     <para>
///     Slots are indexed like <see cref="PacketSubscriptions" />: one catalog per (phase, direction),
///     <see cref="PacketIdentity.Ordinal" /> inside it. A packet nobody expects costs a null check and
///     is dropped; expectations buffer only what they filter in, only while they live.
///     </para>
/// </summary>
public sealed class PacketWaiter : IPacketVisitor, IDisposable
{
    private readonly object?[]?[] _catalogs = new object?[]?[PacketRegistry.CatalogCount];
    private readonly object _gate = new();
    private Exception? _closed;

    /// <summary>
    ///     Opens a wait for T. The queue is subscribed before this returns: send the trigger on the
    ///     next line and nothing can slip through. <paramref name="filter" /> runs on the receive
    ///     loop — keep it cheap and pure; an exception from it faults the handle, not the loop.
    /// </summary>
    public Expectation<T> Expect<T>(Func<T, bool>? filter = null, int capacity = 8,
        OverflowPolicy overflow = OverflowPolicy.DropOldest)
        where T : class, IPacket<T>
    {
        var identity = T.Identity;
        lock (_gate)
        {
            if (_closed is not null)
                throw new PacketStreamClosedException(_closed);

            var slot = CatalogOf(identity.Phase, identity.Direction);
            var catalog = _catalogs[slot];
            if (catalog is null || identity.Ordinal >= catalog.Length)
            {
                var size = Math.Max(identity.Ordinal + 1, PacketRegistry.Catalog(identity.Phase, identity.Direction).Length);
                Array.Resize(ref catalog, size);
                Volatile.Write(ref _catalogs[slot], catalog);
            }

            var list = (ExpectationList<T>?)catalog[identity.Ordinal];
            if (list is null)
                Volatile.Write(ref catalog[identity.Ordinal], list = new ExpectationList<T>());

            var expectation = new Expectation<T>(this, filter, capacity, overflow);
            list.Add(expectation);
            return expectation;
        }
    }

    /// <summary>
    ///     The stream ended: every pending and future await throws
    ///     <see cref="PacketStreamClosedException" /> with <paramref name="error" /> inside.
    ///     Call it from the receive loop's finally.
    /// </summary>
    public void Complete(Exception? error = null)
    {
        lock (_gate)
        {
            if (_closed is not null)
                return;
            _closed = error ?? new PacketStreamClosedException();
            var closed = new PacketStreamClosedException(error);
            foreach (var catalog in _catalogs)
            {
                if (catalog is null)
                    continue;
                foreach (var slot in catalog)
                    (slot as IExpectationList)?.FaultAll(closed);
            }
        }
    }

    /// <summary>Same as <see cref="Complete" /> with an <see cref="ObjectDisposedException" />.</summary>
    public void Dispose() => Complete(new ObjectDisposedException(nameof(PacketWaiter)));

    /// <summary>Receive-loop helper: decode <paramref name="raw" /> and offer it to the open expectations.</summary>
    public void Dispatch(in IncomingPacket raw, int protocolVersion, PacketPhase phase,
        PacketDirection direction = PacketDirection.Clientbound)
    {
        var self = this;
        PacketFlow.Dispatch(in raw, protocolVersion, phase, direction, ref self);
    }

    public void Visit<T>(T packet) where T : class, IPacket<T>
    {
        var identity = T.Identity;
        var catalog = Volatile.Read(ref _catalogs[CatalogOf(identity.Phase, identity.Direction)]);
        if (catalog is null || identity.Ordinal >= catalog.Length)
            return;

        if (Volatile.Read(ref catalog[identity.Ordinal]) is not ExpectationList<T> list)
            return;

        foreach (var expectation in list.Snapshot)
            expectation.Offer(packet);
    }

    public void Unknown(in IncomingPacket raw)
    {
    }

    internal void Remove<T>(Expectation<T> expectation) where T : class, IPacket<T>
    {
        var identity = T.Identity;
        lock (_gate)
        {
            var catalog = _catalogs[CatalogOf(identity.Phase, identity.Direction)];
            if (catalog is not null && identity.Ordinal < catalog.Length &&
                catalog[identity.Ordinal] is ExpectationList<T> list)
                list.Remove(expectation);
        }
    }

    private static int CatalogOf(PacketPhase phase, PacketDirection direction)
        => (int)phase * PacketRegistry.DirectionCount + (int)direction;

    private interface IExpectationList
    {
        void FaultAll(Exception error);
    }

    /// <summary>Copy-on-write: Add/Remove under the waiter gate, Visit reads the snapshot lock-free.</summary>
    private sealed class ExpectationList<T> : IExpectationList where T : class, IPacket<T>
    {
        private Expectation<T>[] _items = [];

        public Expectation<T>[] Snapshot => Volatile.Read(ref _items);

        public void Add(Expectation<T> item)
        {
            var items = _items;
            var next = new Expectation<T>[items.Length + 1];
            items.CopyTo(next, 0);
            next[items.Length] = item;
            Volatile.Write(ref _items, next);
        }

        public void Remove(Expectation<T> item)
        {
            var items = _items;
            var index = Array.IndexOf(items, item);
            if (index < 0)
                return;
            var next = new Expectation<T>[items.Length - 1];
            Array.Copy(items, 0, next, 0, index);
            Array.Copy(items, index + 1, next, index, items.Length - index - 1);
            Volatile.Write(ref _items, next);
        }

        public void FaultAll(Exception error)
        {
            foreach (var item in _items)
                item.Fault(error);
        }
    }
}
