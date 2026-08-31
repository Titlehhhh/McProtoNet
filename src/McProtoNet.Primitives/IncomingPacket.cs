namespace McProtoNet.Primitives;

/// <summary>
/// Represents one packet read from the wire, with the id already parsed and the body already
/// decompressed.
/// </summary>
/// <remarks>
/// <see cref="Body"/> is a window owned by the transport and stays valid only until the next read on the
/// same reader. It must be decoded before the next read and must not be held across an await.
/// </remarks>
public readonly struct IncomingPacket
{
    /// <summary>
    /// The wire id of the packet.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The packet body, without the id.
    /// </summary>
    public readonly ReadOnlyMemory<byte> Body;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncomingPacket"/> structure with the specified id and
    /// body.
    /// </summary>
    /// <param name="id">The wire id of the packet.</param>
    /// <param name="body">The packet body, without the id. The memory is not copied.</param>
    public IncomingPacket(int id, ReadOnlyMemory<byte> body)
    {
        Id = id;
        Body = body;
    }

    /// <summary>
    /// Gets the length of the packet, in bytes, counting the VarInt id and the body.
    /// </summary>
    public long FullLength => Id.GetVarIntLength() + Body.Length;
}
