using System.Runtime.CompilerServices;
using DotNext.Buffers;

namespace McProtoNet.Abstractions;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct OutputPacket(MemoryOwner<byte> owner) : IDisposable
{
    public ReadOnlyMemory<byte> Memory => owner.Memory;

    public ReadOnlySpan<byte> Span => owner.Span;

    public void Dispose()
    {
        owner.Dispose();
    }
}