namespace McProtoNet.Protocol;

/// <summary>
///     What every packet answers when it is held by a plain reference: "which packet are you?".
///     <see cref="IPacket{TSelf}" /> puts <c>Identity</c> on the type (static abstract), so it is
///     reachable only where the concrete type is known statically. A decoded packet that left the
///     dispatcher — an element of a packet stream, a value in a switch — has lost that type, and
///     without this interface its only common type is <see cref="object" />.
///     Generated packets implement it explicitly (<c>PacketIdentity IPacket.Identity => Identity;</c>):
///     an explicit implementation is not a named member of the class, so the instance property and
///     the static one coexist.
///     <see cref="IPacket{TSelf}" /> does NOT inherit this interface — the static member cannot
///     satisfy the instance one, so inheriting would break every hand-written packet.
/// </summary>
public interface IPacket
{
    /// <summary>Identity of the packet this instance is — the same value as the type's static one.</summary>
    PacketIdentity Identity { get; }
}

/// <summary>
///     A protocol type that is a top-level packet: carries identity and wire ids.
///     Nested protocol types (Slot, LoginSignature, ...) stay plain <see cref="IProtocolType{TSelf}" />.
///     Packets are classes (owner decision 2026-08-08): one allocation per packet, no boxing anywhere.
/// </summary>
public interface IPacket<TSelf> : IProtocolType<TSelf> where TSelf : class, IPacket<TSelf>
{
    static abstract PacketIdentity Identity { get; }

    /// <summary>False when the packet does not exist on the given protocol version.</summary>
    static abstract bool TryGetPacketId(int protocolVersion, out int id);
}
