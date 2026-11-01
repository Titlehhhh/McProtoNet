namespace McProtoNet.Protocol;

/// <summary>
/// Defines a protocol type that is a top-level packet and therefore carries an identity and per-version
/// wire ids.
/// </summary>
/// <typeparam name="TSelf">The packet type that implements the interface.</typeparam>
/// <remarks>
/// Nested protocol types, such as a slot or a login signature, implement
/// <see cref="IProtocolType{TSelf}"/> instead. Packets are classes, so a packet costs one allocation
/// and dispatch does not box.
/// </remarks>
public interface IPacket<TSelf> : IProtocolType<TSelf> where TSelf : class, IPacket<TSelf>
{
    /// <summary>
    /// Gets the identity of the packet type.
    /// </summary>
    static abstract PacketIdentity Identity { get; }

    /// <summary>
    /// Attempts to get the wire id of the packet for the specified protocol version.
    /// </summary>
    /// <param name="protocolVersion">The protocol version to look the wire id up for.</param>
    /// <param name="id">When this method returns, contains the wire id of the packet for the specified
    /// protocol version, if the packet exists on that version; otherwise, the value is unspecified.</param>
    /// <returns>
    /// <see langword="true"/> if the packet exists on the specified protocol version; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    static abstract bool TryGetPacketId(int protocolVersion, out int id);
}
