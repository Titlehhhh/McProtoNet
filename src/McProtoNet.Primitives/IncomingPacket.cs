namespace McProtoNet.Primitives;

/// <summary>
/// Represents one packet read from the wire, with the id already parsed and the body already
/// decompressed.
/// </summary>
/// <remarks>
/// A packet returned by a single read owns one reference to the <see cref="PooledBlock"/> behind its
/// body and must be disposed: <see cref="Dispose"/> gives the reference back and empties
/// <see cref="Body"/>. A packet handed out by an enumeration is borrowed (<see cref="Borrow"/>): the
/// enumerator owns it and releases it at its next step, and <see cref="Dispose"/> on the borrowed copy
/// does nothing. <see cref="Retain"/> hands out a copy with a reference of its own, which is the way
/// to keep a packet past the step that received it. Like <see cref="MemoryOwner{T}"/>, this is a
/// mutable structure: copy it only to transfer ownership. Disposing one instance twice has no effect
/// the second time, but disposing two owning copies of one packet releases the block twice, which
/// throws.
/// </remarks>
public struct IncomingPacket : IDisposable
{
    /// <summary>
    /// The wire id of the packet.
    /// </summary>
    public readonly int Id;

    private ReadOnlyMemory<byte> _body;
    private PooledBlock? _block;
    private bool _owning;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncomingPacket"/> structure over memory that the
    /// packet does not own.
    /// </summary>
    /// <param name="id">The wire id of the packet.</param>
    /// <param name="body">The packet body, without the id. The memory is not copied.</param>
    public IncomingPacket(int id, ReadOnlyMemory<byte> body)
    {
        Id = id;
        _body = body;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncomingPacket"/> structure over a window of a
    /// pooled block.
    /// </summary>
    /// <param name="id">The wire id of the packet.</param>
    /// <param name="block">The block that holds the body. The packet takes over one reference that the
    /// caller holds; the caller must not release it.</param>
    /// <param name="offset">The offset of the body in the block.</param>
    /// <param name="length">The length of the body.</param>
    /// <exception cref="ArgumentNullException"><paramref name="block"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The window does not fit in the block.</exception>
    public IncomingPacket(int id, PooledBlock block, int offset, int length)
    {
        ArgumentNullException.ThrowIfNull(block);
        Id = id;
        _block = block;
        _body = block.Array.AsMemory(offset, length);
        _owning = true;
    }

    private IncomingPacket(int id, PooledBlock? block, ReadOnlyMemory<byte> body, bool owning)
    {
        Id = id;
        _block = block;
        _body = body;
        _owning = owning;
    }

    /// <summary>
    /// Gets the packet body, without the id. Empty once the packet is disposed.
    /// </summary>
    public readonly ReadOnlyMemory<byte> Body => _body;

    /// <summary>
    /// Gets the length of the packet, in bytes, counting the VarInt id and the body.
    /// </summary>
    public readonly long FullLength => Id.GetVarIntLength() + _body.Length;

    /// <summary>
    /// Returns a copy of the packet that holds a reference of its own, so it stays valid after this
    /// instance is disposed or released by its enumerator.
    /// </summary>
    /// <returns>A packet over the same body that must be disposed separately.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    public readonly IncomingPacket Retain()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(IncomingPacket));
        _block?.Retain();
        return new IncomingPacket(Id, _block, _body, owning: _block is not null);
    }

    /// <summary>
    /// Returns a copy of the packet that holds no reference: it is valid as long as this instance is,
    /// its <see cref="Dispose"/> does nothing, and its <see cref="Retain"/> works.
    /// </summary>
    /// <returns>A borrowed packet over the same body.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    public readonly IncomingPacket Borrow()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(IncomingPacket));
        return new IncomingPacket(Id, _block, _body, owning: false);
    }

    /// <summary>
    /// Releases the reference this instance holds to the block behind <see cref="Body"/>, if it holds
    /// one, and empties <see cref="Body"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _body = default;

        var block = _block;
        _block = null;
        if (_owning) block?.Release();
    }
}
