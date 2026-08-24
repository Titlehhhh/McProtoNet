using McProtoNet.Primitives;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Represents the packets of every whole frame that one read produced.
/// </summary>
/// <remarks>
/// The batch and every packet body it hands out stay valid only until the next batch is read from the
/// same reader. This rule is not enforced.
/// </remarks>
public readonly struct PacketBatch
{
    private readonly BufferedPacketReader? _reader;
    private readonly int _count;
    private readonly bool _completed;

    internal PacketBatch(BufferedPacketReader? reader, int count, bool completed)
    {
        _reader = reader;
        _count = count;
        _completed = completed;
    }

    /// <summary>
    /// Gets the number of packets in the batch.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Gets a value indicating whether the stream has ended.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the stream ended and no further packets will arrive; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsCompleted => _completed;

    /// <summary>
    /// Returns an enumerator that iterates through the packets of the batch.
    /// </summary>
    /// <returns>An <see cref="Enumerator"/> for the batch.</returns>
    public Enumerator GetEnumerator() => new(_reader, _count);

    /// <summary>
    /// Enumerates the packets of a <see cref="PacketBatch"/>.
    /// </summary>
    public struct Enumerator
    {
        private readonly BufferedPacketReader? _reader;
        private readonly int _count;
        private int _index;

        internal Enumerator(BufferedPacketReader? reader, int count)
        {
            _reader = reader;
            _count = count;
            _index = -1;
        }

        /// <summary>
        /// Gets the packet at the current position of the enumerator.
        /// </summary>
        public IncomingPacket Current => _reader!.GetPacket(_index);

        /// <summary>
        /// Advances the enumerator to the next packet of the batch.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator moved to the next packet; otherwise,
        /// <see langword="false"/>.</returns>
        public bool MoveNext() => ++_index < _count;
    }
}
