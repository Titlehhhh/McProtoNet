using McProtoNet.Primitives;

namespace McProtoNet.Transport.Framing;

/// <summary>
///     Every whole frame the reader had after one await. The batch and every body window it hands
///     out live until the next <see cref="BufferedPacketReader.ReadBatchAsync" /> on the same reader —
///     a written rule, not a checked one.
/// </summary>
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

    /// <summary>Number of packets in the batch.</summary>
    public int Count => _count;

    /// <summary>True when the stream ended: no more packets will ever arrive.</summary>
    public bool IsCompleted => _completed;

    public Enumerator GetEnumerator() => new(_reader, _count);

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

        public IncomingPacket Current => _reader!.GetPacket(_index);

        public bool MoveNext() => ++_index < _count;
    }
}
