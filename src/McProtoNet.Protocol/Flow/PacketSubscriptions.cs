using McProtoNet.Net;

namespace McProtoNet.Protocol;

public delegate void PacketHandler<T>(T packet) where T : class, IPacket<T>;

/// <summary>
///     Subscription facade in front of an <see cref="IPacketVisitor" />-driven dispatch loop.
///     Slots are indexed by <see cref="PacketIdentity.Ordinal" />; the array grows on demand in
///     <see cref="On{T}" /> because there is no registry yet to pre-size it from. Visit is the
///     hot path: bounds check, index, pattern-cast — no allocation, no reflection.
/// </summary>
public sealed class PacketSubscriptions : IPacketVisitor
{
    private Delegate?[] _slots = [];

    /// <summary>Registers or replaces the handler for T. Growing the slot array is a cold-path cost.</summary>
    public void On<T>(PacketHandler<T> handler) where T : class, IPacket<T>
    {
        var ordinal = T.Identity.Ordinal;
        if (ordinal >= _slots.Length)
            Array.Resize(ref _slots, ordinal + 1);

        _slots[ordinal] = handler;
    }

    public void Visit<T>(T packet) where T : class, IPacket<T>
    {
        var ordinal = T.Identity.Ordinal;
        if (ordinal >= _slots.Length)
            return;

        if (_slots[ordinal] is PacketHandler<T> handler)
            handler(packet);
    }

    public void Unknown(in InputPacket raw)
    {
    }
}
