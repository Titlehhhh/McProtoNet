using System.Runtime.CompilerServices;
namespace McProtoNet.Primitives;

/// <summary>
/// Represents a packet to be written to the wire, backed by a pooled buffer.
/// </summary>
/// <param name="owner">The pooled buffer that holds the packet data. This instance takes ownership of
/// it.</param>
/// <remarks>
/// Disposing the instance returns the buffer to the pool. <see cref="Memory"/> and <see cref="Span"/>
/// are valid only until the instance is disposed, and the instance must be disposed exactly once.
/// </remarks>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct OutgoingPacket(MemoryOwner<byte> owner) : IDisposable
{
    /// <summary>
    /// Gets the packet data as read-only memory.
    /// </summary>
    public ReadOnlyMemory<byte> Memory => owner.Memory;

    /// <summary>
    /// Gets the packet data as a read-only span.
    /// </summary>
    public ReadOnlySpan<byte> Span => owner.Span;

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="OutgoingPacket"/> structure by
    /// returning its buffer to the pool.
    /// </summary>
    public void Dispose()
    {
        owner.Dispose();
    }
}
