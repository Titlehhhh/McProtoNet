using System.Runtime.CompilerServices;
namespace McProtoNet.Primitives;

/// <summary>
/// Represents a packet to be written to the wire, backed by a pooled buffer.
/// </summary>
/// <param name="owner">The pooled buffer that holds the packet data. This instance takes ownership of
/// it.</param>
/// <remarks>
/// Disposing the instance returns the buffer to the pool. <see cref="Memory"/> and <see cref="Span"/>
/// are valid only until the instance is disposed. Like the <see cref="MemoryOwner{T}"/> it holds, this
/// is a mutable structure, so a copy must be made only to transfer ownership; a copy that outlives the
/// transfer still holds the buffer and returns it a second time, which corrupts the pool. Disposing one
/// instance twice has no effect the second time.
/// </remarks>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct OutgoingPacket(MemoryOwner<byte> owner) : IDisposable
{
    /// <summary>
    /// Gets the packet data as read-only memory.
    /// </summary>
    public readonly ReadOnlyMemory<byte> Memory => owner.Memory;

    /// <summary>
    /// Gets the packet data as a read-only span.
    /// </summary>
    public readonly ReadOnlySpan<byte> Span => owner.Span;

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="OutgoingPacket"/> structure by
    /// returning its buffer to the pool.
    /// </summary>
    public void Dispose()
    {
        owner.Dispose();
    }
}
