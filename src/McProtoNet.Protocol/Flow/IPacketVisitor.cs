using McProtoNet.Net;

namespace McProtoNet.Protocol;

/// <summary>
///     Consumer of a dispatched packet stream. <see cref="Visit{T}" /> receives the decoded
///     packet with T statically known at each generated call site — no boxing, no reflection.
///     <see cref="Unknown" /> receives packets whose id the registry cannot map: a normal
///     condition of the stream, not an error.
/// </summary>
public interface IPacketVisitor
{
    void Visit<T>(T packet) where T : class, IPacket<T>;

    void Unknown(in InputPacket raw);
}
