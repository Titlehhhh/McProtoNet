namespace McProtoNet.Primitives;

/// <summary>
///     One packet off the wire: id already parsed, body after decompression.
///     <see cref="Body" /> is a window owned by the transport — it stays valid only until the
///     next read on the same reader. Decode before the next read; never hold it across an await.
/// </summary>
public readonly struct IncomingPacket
{
    public readonly int Id;
    public readonly ReadOnlyMemory<byte> Body;

    public IncomingPacket(int id, ReadOnlyMemory<byte> body)
    {
        Id = id;
        Body = body;
    }

    /// <summary>Length of the packet as the varint id plus the body.</summary>
    public long FullLength => Id.GetVarIntLength() + Body.Length;
}
