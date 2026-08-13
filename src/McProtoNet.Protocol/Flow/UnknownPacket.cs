namespace McProtoNet.Protocol;

/// <summary>
///     Element of a packet stream for a wire id the registry cannot map in the current
///     (phase, direction). Carries the id, the phase and the direction — deliberately no body:
///     the raw bytes live in a transport window that dies on the next read, and the typed floor
///     does not hand out raw windows. Consumers who need the body read the raw floor instead.
///     An unknown id is a normal condition of the stream, not an error.
///     It implements <see cref="IPacket" /> so that a switch over the stream stays one world:
///     mapped and unmapped packets share a type.
/// </summary>
public sealed class UnknownPacket : IPacket
{
    /// <summary>
    ///     Ordinal reported by <see cref="Identity" />. Catalog ordinals are dense from 0 and every
    ///     catalog is far smaller, so this value belongs to no packet and marks "outside the catalog".
    /// </summary>
    public const ushort UnmappedOrdinal = ushort.MaxValue;

    /// <summary>
    ///     Key reported by <see cref="Identity" />. Manifest keys are dotted
    ///     (<c>play.toClient.teams</c>), so this one collides with none of them.
    /// </summary>
    public const string UnmappedKey = "unknown";

    /// <summary>Name reported by <see cref="Identity" />.</summary>
    public const string UnmappedName = "Unknown";

    /// <summary>Initializes an unknown-packet marker.</summary>
    /// <param name="id">The wire id that had no mapping.</param>
    /// <param name="phase">The protocol phase in which the packet arrived.</param>
    /// <param name="direction">The direction in which the packet travelled.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="phase" /> or <paramref name="direction" /> is not a defined value.
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

    /// <summary>Gets the wire id that had no mapping in the registry.</summary>
    public int Id { get; }

    /// <summary>Gets the protocol phase in which the packet arrived.</summary>
    public PacketPhase Phase { get; }

    /// <summary>Gets the direction in which the packet travelled.</summary>
    public PacketDirection Direction { get; }

    /// <summary>
    ///     Identity of an unmapped packet. Phase and direction are the real ones; key, name and
    ///     ordinal are the constants above, because an unmapped id has none of the three. Read
    ///     <see cref="Id" /> for the only wire fact this packet carries.
    /// </summary>
    public PacketIdentity Identity => new(UnmappedKey, UnmappedName, Phase, Direction, UnmappedOrdinal);
}
