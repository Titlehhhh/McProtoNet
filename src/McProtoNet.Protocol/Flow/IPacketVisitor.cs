using McProtoNet.Primitives;
namespace McProtoNet.Protocol;

/// <summary>
/// Defines a consumer of a dispatched packet stream.
/// </summary>
/// <remarks>
/// The type parameter of <see cref="Visit{T}"/> is statically known at every generated call site,
/// so dispatch uses neither boxing nor reflection.
/// </remarks>
public interface IPacketVisitor
{
    /// <summary>
    /// Receives a decoded packet.
    /// </summary>
    /// <typeparam name="T">The type of the decoded packet.</typeparam>
    /// <param name="packet">The decoded packet.</param>
    void Visit<T>(T packet) where T : class, IPacket<T>;

    /// <summary>
    /// Receives a packet whose identifier the registry cannot map to a packet type.
    /// </summary>
    /// <param name="raw">The undecoded packet as it was read from the connection.</param>
    /// <remarks>
    /// An unmapped identifier is a normal condition of the stream, not an error.
    /// </remarks>
    void Unknown(in IncomingPacket raw);
}
