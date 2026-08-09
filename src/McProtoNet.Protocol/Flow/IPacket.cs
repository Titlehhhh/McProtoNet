namespace McProtoNet.Protocol;

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
