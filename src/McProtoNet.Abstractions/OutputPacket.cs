using System.Runtime.CompilerServices;
using DotNext.Buffers;

namespace McProtoNet.Abstractions;

/// <summary>
/// 
/// </summary>
/// <param name="owner"></param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct OutputPacket(MemoryOwner<byte> owner) : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    public ReadOnlyMemory<byte> Memory => owner.Memory;

    /// <summary>
    /// 
    /// </summary>
    public ReadOnlySpan<byte> Span => owner.Span;

    /// <inheritdoc />
    public void Dispose()
    {
        owner.Dispose();
    }
}