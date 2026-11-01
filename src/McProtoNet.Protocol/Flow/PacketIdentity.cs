namespace McProtoNet.Protocol;

/// <summary>
/// Represents the identity of a packet type.
/// </summary>
/// <param name="Key">The manifest key of the packet, such as <c>login.toServer.login_start</c>.</param>
/// <param name="Name">The name of the packet type.</param>
/// <param name="Phase">The connection phase the packet belongs to.</param>
/// <param name="Direction">The direction the packet travels in.</param>
/// <param name="Ordinal">The dense index of the packet inside its catalog, ordered by key.</param>
/// <remarks>
/// A catalog is one pair of phase and direction. The ordinal is dense only within a single catalog,
/// so it does not identify a packet on its own. It is stable across builds and is used to index
/// dispatch tables and subscription slots.
/// </remarks>
public readonly record struct PacketIdentity(
    string Key,
    string Name,
    PacketPhase Phase,
    PacketDirection Direction,
    ushort Ordinal);
