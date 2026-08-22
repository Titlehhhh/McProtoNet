using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

public delegate void PacketHandler<T>(T packet) where T : class, IPacket<T>;

/// <summary>
///     Subscription facade in front of an <see cref="IPacketVisitor" />-driven dispatch loop.
///     <see cref="PacketIdentity.Ordinal" /> is dense only inside one (phase, direction) catalog, so
///     the ordinal alone is not a key: <c>play.toClient</c> and <c>login.toClient</c> both start at 0.
///     Slots therefore hang off one catalog per (phase, direction), and the ordinal indexes inside it.
///     Each catalog grows on demand in <see cref="On{T}" />; only the number of catalogs is known up
///     front, from the generated <see cref="PacketRegistry.CatalogCount" />. Visit is the hot path:
///     two bounds checks, two indexes, pattern-cast — no allocation, no reflection.
/// </summary>
public sealed class PacketSubscriptions : IPacketVisitor
{
    private readonly Delegate?[]?[] _catalogs = new Delegate?[]?[PacketRegistry.CatalogCount];

    /// <summary>Registers or replaces the handler for T. Growing a catalog is a cold-path cost.</summary>
    public void On<T>(PacketHandler<T> handler) where T : class, IPacket<T>
    {
        var identity = T.Identity;
        var slot = CatalogOf(identity.Phase, identity.Direction);
        var catalog = _catalogs[slot] ?? [];

        if (identity.Ordinal >= catalog.Length)
            Array.Resize(ref catalog, identity.Ordinal + 1);

        catalog[identity.Ordinal] = handler;
        _catalogs[slot] = catalog;
    }

    public void Visit<T>(T packet) where T : class, IPacket<T>
    {
        var identity = T.Identity;
        var catalog = _catalogs[CatalogOf(identity.Phase, identity.Direction)];
        if (catalog is null || identity.Ordinal >= catalog.Length)
            return;

        if (catalog[identity.Ordinal] is PacketHandler<T> handler)
            handler(packet);
    }

    public void Unknown(in IncomingPacket raw)
    {
    }

    private static int CatalogOf(PacketPhase phase, PacketDirection direction)
    {
        return (int)phase * PacketRegistry.DirectionCount + (int)direction;
    }
}
