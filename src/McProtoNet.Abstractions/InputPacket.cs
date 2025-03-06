using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Abstractions;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct InputPacket : IDisposable
{
    public readonly int Id;
    public readonly ReadOnlyMemory<byte> Data;


    private MemoryOwner<byte> owner;
    private readonly bool isMemoryPool;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="owner"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputPacket(int id, MemoryOwner<byte> owner)
    {
        this.owner = owner;
        Id = id;
        Data = this.owner.Memory;
       
       
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="offset"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputPacket(MemoryOwner<byte> owner, int offset = 0)
    {
        this.owner = owner;
        Memory<byte> mainData = this.owner.Memory.Slice(offset);

        Id = Extensions.ReadVarInt(mainData.Span, out var offsetId);
        Data = mainData.Slice(offsetId);
      
    }


    public void Dispose()
    {
        if (isMemoryPool)
            owner.Dispose();
    }
}