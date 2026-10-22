using McProtoNet.Primitives;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Represents the packets of every whole frame that one read produced.
/// </summary>
/// <remarks>
/// The batch itself owns nothing and can be enumerated only until the next batch is read from the
/// same reader; this rule is not enforced. The enumerator owns the packet it stands on and releases
/// it at its next step or at the end of the enumeration; the packet it hands out is borrowed. To keep
/// a packet past the step, call <see cref="IncomingPacket.Retain"/>.
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
    /// <remarks>
    /// The enumerator holds one reference to the block behind the packet it stands on and releases
    /// it in <see cref="MoveNext"/> and <see cref="Dispose"/>.
    /// </remarks>
    public struct Enumerator : IDisposable
    {
        private readonly BufferedPacketReader? _reader;
        private readonly int _count;
        private int _index;
        private IncomingPacket _owned;

        internal Enumerator(BufferedPacketReader? reader, int count)
        {
            _reader = reader;
            _count = count;
            _index = -1;
        }

        /// <summary>
        /// Gets a borrowed copy of the packet at the current position of the enumerator. It is valid
        /// until the next call to <see cref="MoveNext"/>.
        /// </summary>
        public readonly IncomingPacket Current => _owned.Borrow();

        /// <summary>
        /// Releases the packet the enumerator stands on and advances to the next packet of the batch.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator moved to the next packet; otherwise,
        /// <see langword="false"/>.</returns>
        public bool MoveNext()
        {
            // the next packet is taken before the previous one is let go: once the reader is disposed
            // the enumerator may be the only thing keeping the block alive
            var previous = _owned;
            if (++_index >= _count)
            {
                _owned = default;
                previous.Dispose();
                return false;
            }

            _owned = _reader!.GetPacket(_index);
            previous.Dispose();
            return true;
        }

        /// <summary>
        /// Releases the packet the enumerator stands on.
        /// </summary>
        public void Dispose() => _owned.Dispose();
    }
}
