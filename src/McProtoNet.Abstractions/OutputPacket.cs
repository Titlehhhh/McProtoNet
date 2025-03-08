using System.Runtime.CompilerServices;
using DotNext.Buffers;

namespace McProtoNet.Abstractions;

/// <summary>
/// Represents an output packet used in the Minecraft protocol.
/// This structure holds the packet data and provides access to it in memory and span formats.
/// </summary>
/// <param name="owner">The memory owner containing the packet data.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct OutputPacket(MemoryOwner<byte> owner) : IDisposable
{
    /// <summary>
    /// Gets the read-only memory containing the packet data.
    /// </summary>
    public ReadOnlyMemory<byte> Memory => owner.Memory;

    /// <summary>
    /// Gets the read-only span representing the packet data.
    /// </summary>
    public ReadOnlySpan<byte> Span => owner.Span;

    /// <summary>
    /// Releases the resources used by this packet.
    /// </summary>
    public void Dispose()
    {
        owner.Dispose();
    }
}
