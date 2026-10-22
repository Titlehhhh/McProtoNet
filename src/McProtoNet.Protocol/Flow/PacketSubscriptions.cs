using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Represents the method that handles a decoded packet of the type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The packet type that is handled.</typeparam>
/// <param name="packet">The decoded packet.</param>
public delegate void PacketHandler<T>(T packet) where T : class, IPacket<T>;

/// <summary>
/// Provides a subscription facade over an <see cref="IPacketVisitor"/> dispatch loop that routes each
/// packet to the handler registered for its type.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="PacketIdentity.Ordinal"/> is dense only inside one catalog, which is one pair of phase and
/// direction, so the ordinal alone is not a key. Handler slots are held in one catalog per pair, and the
/// ordinal indexes inside that catalog. The number of catalogs is known from
/// <see cref="PacketRegistry.CatalogCount"/>; each catalog grows on demand in <see cref="On{T}"/>.
/// </para>
/// <para>
/// <see cref="Visit{T}"/> performs two bounds checks, two array accesses and a pattern-matching cast.
/// It does not allocate and does not use reflection.
/// </para>
/// </remarks>
public sealed class PacketSubscriptions : IPacketVisitor
{
    private readonly Delegate?[]?[] _catalogs = new Delegate?[]?[PacketRegistry.CatalogCount];

    /// <summary>
    /// Registers the handler for the packet type <typeparamref name="T"/>, replacing any handler that is
    /// already registered for it.
    /// </summary>
    /// <typeparam name="T">The packet type to handle.</typeparam>
    /// <param name="handler">The handler to invoke for each packet of the type.</param>
    /// <remarks>Growing a catalog to fit the ordinal of the packet type is a cold-path cost.</remarks>
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

    /// <inheritdoc />
    /// <remarks>
    /// If no handler is registered for the packet type, the packet is ignored.
    /// </remarks>
    public void Visit<T>(T packet) where T : class, IPacket<T>
    {
        var identity = T.Identity;
        var catalog = _catalogs[CatalogOf(identity.Phase, identity.Direction)];
        if (catalog is null || identity.Ordinal >= catalog.Length)
            return;

        if (catalog[identity.Ordinal] is PacketHandler<T> handler)
            handler(packet);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation ignores the packet.
    /// </remarks>
    public void Unknown(in IncomingPacket raw)
    {
    }

    private static int CatalogOf(PacketPhase phase, PacketDirection direction)
    {
        return (int)phase * PacketRegistry.DirectionCount + (int)direction;
    }
}
