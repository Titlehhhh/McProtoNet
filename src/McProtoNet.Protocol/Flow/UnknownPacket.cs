namespace McProtoNet.Protocol;

/// <summary>
/// Represents a packet whose wire id has no mapping in the registry for the current phase and
/// direction.
/// </summary>
/// <remarks>
/// <para>
/// The instance carries the wire id, the phase and the direction, but no body. The raw bytes stay
/// with the raw packet that was dispatched, so they are not copied here; a consumer that needs the
/// body keeps that packet through its <c>Retain</c> method instead.
/// </para>
/// <para>
/// An unmapped id is a normal condition of the stream and is not an error. The type implements
/// <see cref="IPacket"/> so that mapped and unmapped packets share one type in a switch.
/// </para>
/// </remarks>
public sealed class UnknownPacket : IPacket
{
    /// <summary>The ordinal reported by <see cref="Identity"/>.</summary>
    /// <remarks>
    /// Catalog ordinals are dense from zero and every catalog is far smaller than this value, so the
    /// value belongs to no packet and marks an identity outside the catalog.
    /// </remarks>
    public const ushort UnmappedOrdinal = ushort.MaxValue;

    /// <summary>The manifest key reported by <see cref="Identity"/>.</summary>
    /// <remarks>
    /// Manifest keys are dotted, such as <c>play.toClient.teams</c>, so this key collides with none of
    /// them.
    /// </remarks>
    public const string UnmappedKey = "unknown";

    /// <summary>The packet name reported by <see cref="Identity"/>.</summary>
    public const string UnmappedName = "Unknown";

    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownPacket"/> class with the specified wire id,
    /// phase and direction.
    /// </summary>
    /// <param name="id">The wire id that had no mapping.</param>
    /// <param name="phase">The protocol phase in which the packet arrived.</param>
    /// <param name="direction">The direction in which the packet travelled.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="phase"/> is not a defined value.
    /// -or-
    /// <paramref name="direction"/> is not a defined value.
    /// </exception>
    public UnknownPacket(int id, PacketPhase phase, PacketDirection direction)
    {
        if (phase > PacketPhase.Play)
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown protocol phase.");
        if (direction > PacketDirection.Serverbound)
            throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown packet direction.");
        Id = id;
        Phase = phase;
        Direction = direction;
    }

    /// <summary>
    /// Gets the wire id that had no mapping in the registry.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the protocol phase in which the packet arrived.
    /// </summary>
    public PacketPhase Phase { get; }

    /// <summary>
    /// Gets the direction in which the packet travelled.
    /// </summary>
    public PacketDirection Direction { get; }

    /// <summary>
    /// Gets the identity of the unmapped packet.
    /// </summary>
    /// <value>
    /// A <see cref="PacketIdentity"/> whose phase and direction are those of the received packet and
    /// whose key, name and ordinal are <see cref="UnmappedKey"/>, <see cref="UnmappedName"/> and
    /// <see cref="UnmappedOrdinal"/>.
    /// </value>
    /// <remarks>
    /// An unmapped id has no key, name or ordinal of its own. <see cref="Id"/> holds the only wire
    /// fact this packet carries.
    /// </remarks>
    public PacketIdentity Identity => new(UnmappedKey, UnmappedName, Phase, Direction, UnmappedOrdinal);
}
