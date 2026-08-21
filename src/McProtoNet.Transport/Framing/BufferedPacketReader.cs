using System.Buffers;
using System.Runtime.CompilerServices;
using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
///     Streaming reader: one pooled buffer filled in big reads, decrypted in place right after the
///     read, and split into frames where it lies. A batch is every whole frame the buffer holds after
///     one await. Compressed frames are inflated into a grow-only arena that is reset on every batch.
/// </summary>
/// <remarks>
///     One owner: concurrent reads are not allowed. Cipher and compression threshold are fixed at
///     construction. A batch and its bodies live until the next <see cref="ReadBatchAsync" />.
/// </remarks>
internal sealed class BufferedPacketReader : IDisposable
{
    private const int DefaultCapacity = 64 * 1024;

    /// <summary>Wire-supplied lengths above this are treated as a broken stream, not honoured.</summary>
    public const int MaxFrameLength = 32 * 1024 * 1024;

    private readonly Stream _stream;
    private readonly PacketCipher? _cipher;
    private readonly int _compressionThreshold;

    private byte[] _buffer;
    private int _start;
    private int _end;

    private byte[] _arena = [];
    private int _arenaUsed;

    private Frame[] _frames = new Frame[64];
    private int _count;
    private int _needed = 1;

    private int _reading;
    private bool _disposed;
    private bool _eof;

    public BufferedPacketReader(Stream stream, int compressionThreshold = -1, PacketCipher? cipher = null,
        int initialCapacity = DefaultCapacity)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
        _compressionThreshold = compressionThreshold;
        _cipher = cipher;
        _buffer = ArrayPool<byte>.Shared.Rent(Math.Max(initialCapacity, 1024));
    }

    /// <summary>Negative means no compression envelope. Fixed for the life of the reader.</summary>
    public int CompressionThreshold => _compressionThreshold;

    /// <summary>True when a decryptor is attached.</summary>
    public bool IsEncrypted => _cipher is not null;

    /// <summary>Bytes already read but not yet handed out as packets.</summary>
    public int BufferedBytes => _end - _start;

    /// <summary>
    ///     Reads until at least one whole frame is available and returns all of them. An empty batch
    ///     with <see cref="PacketBatch.IsCompleted" /> means end of stream.
    /// </summary>
    public ValueTask<PacketBatch> ReadBatchAsync(CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (Interlocked.Exchange(ref _reading, 1) == 1) ThrowHelper.ThrowConcurrentRead();

        try
        {
            token.ThrowIfCancellationRequested();

            _arenaUsed = 0;
            _count = 0;

            ParseFrames();
            if (_count > 0)
            {
                Volatile.Write(ref _reading, 0);
                return new ValueTask<PacketBatch>(new PacketBatch(this, _count, false));
            }

            if (_eof)
            {
                if (_end > _start) ThrowHelper.ThrowTruncatedFrame();

                Volatile.Write(ref _reading, 0);
                return new ValueTask<PacketBatch>(new PacketBatch(this, 0, true));
            }
        }
        catch
        {
            Volatile.Write(ref _reading, 0);
            throw;
        }

        return FillAndParseAsync(token);
    }

    /// <summary>Batches flattened into one sequence. Enumeration ends at end of stream.</summary>
    public async IAsyncEnumerable<IncomingPacket> ReadPacketsAsync(
        [EnumeratorCancellation] CancellationToken token = default)
    {
        while (true)
        {
            var batch = await ReadBatchAsync(token).ConfigureAwait(false);
            if (batch.Count == 0)
            {
                if (batch.IsCompleted) yield break;
                continue;
            }

            foreach (var packet in batch) yield return packet;
        }
    }

    private async ValueTask<PacketBatch> FillAndParseAsync(CancellationToken token)
    {
        try
        {
            while (true)
            {
                MakeRoom();

                var read = await _stream.ReadAsync(_buffer.AsMemory(_end), token).ConfigureAwait(false);
                if (read == 0)
                {
                    _eof = true;
                    if (_end > _start) ThrowHelper.ThrowTruncatedFrame();

                    return new PacketBatch(this, 0, true);
                }

                _cipher?.Transform(_buffer.AsSpan(_end, read));
                _end += read;

                ParseFrames();
                if (_count > 0) return new PacketBatch(this, _count, false);
            }
        }
        finally
        {
            Volatile.Write(ref _reading, 0);
        }
    }

    private void MakeRoom()
    {
        // room at the tail to read into, and the frame being assembled still fits where it lies
        if (_end < _buffer.Length && _buffer.Length - _start >= _needed) return;

        if (_start > 0)
        {
            if (_end > _start) _buffer.AsSpan(_start, _end - _start).CopyTo(_buffer);
            _end -= _start;
            _start = 0;
        }

        if (_buffer.Length < _needed) Grow(_needed);
        else if (_buffer.Length == _end) Grow(_buffer.Length * 2);
    }

    private void Grow(int wanted)
    {
        var next = ArrayPool<byte>.Shared.Rent(Math.Max(wanted, _buffer.Length + 1));
        _buffer.AsSpan(_start, _end - _start).CopyTo(next);
        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = next;
        _end -= _start;
        _start = 0;
    }

    private void ParseFrames()
    {
        while (true)
        {
            var available = _end - _start;
            if (available <= 0)
            {
                _needed = 1;
                return;
            }

            if (!_buffer.AsSpan(_start, available).TryReadVarInt(out var length, out var lengthBytes))
            {
                _needed = available + 1;
                return;
            }

            if (length <= 0 || length > MaxFrameLength) ThrowHelper.ThrowInvalidFrameLength(length);

            if (available - lengthBytes < length)
            {
                _needed = lengthBytes + length;
                return;
            }

            AddFrame(_start + lengthBytes, length);
            _start += lengthBytes + length;
        }
    }

    private void AddFrame(int offset, int length)
    {
        if (_compressionThreshold < 0)
        {
            var id = _buffer.AsSpan(offset, length).ReadVarInt(out var idLength);
            Append(id, offset + idLength, length - idLength, fromArena: false);
            return;
        }

        var sizeUncompressed = _buffer.AsSpan(offset, length).ReadVarInt(out var sizeLength);
        if (sizeUncompressed <= 0)
        {
            var plainLength = length - sizeLength;
            if (plainLength <= 0) ThrowHelper.ThrowEmptyEnvelope();

            var id = _buffer.AsSpan(offset + sizeLength, plainLength).ReadVarInt(out var idLength);
            Append(id, offset + sizeLength + idLength, plainLength - idLength, fromArena: false);
            return;
        }

        if (sizeUncompressed > MaxFrameLength || length - sizeLength <= 0)
            ThrowHelper.ThrowInvalidUncompressedSize(sizeUncompressed);

        var target = ArenaAllocate(sizeUncompressed);
        PacketStreamReader.DecompressCore(
            _buffer.AsSpan(offset + sizeLength, length - sizeLength),
            _arena.AsSpan(target, sizeUncompressed));

        var packetId = _arena.AsSpan(target, sizeUncompressed).ReadVarInt(out var packetIdLength);
        Append(packetId, target + packetIdLength, sizeUncompressed - packetIdLength, fromArena: true);
    }

    private int ArenaAllocate(int size)
    {
        if (_arena.Length - _arenaUsed < size)
        {
            var wanted = Math.Max(_arenaUsed + size, Math.Max(_arena.Length * 2, DefaultCapacity));
            var next = ArrayPool<byte>.Shared.Rent(wanted);
            if (_arenaUsed > 0) _arena.AsSpan(0, _arenaUsed).CopyTo(next);
            if (_arena.Length > 0) ArrayPool<byte>.Shared.Return(_arena);
            _arena = next;
        }

        var offset = _arenaUsed;
        _arenaUsed += size;
        return offset;
    }

    private void Append(int id, int offset, int length, bool fromArena)
    {
        if (length < 0) ThrowHelper.ThrowIdPastFrameEnd();
        if (_count == _frames.Length) Array.Resize(ref _frames, _frames.Length * 2);
        _frames[_count++] = new Frame(id, offset, length, fromArena);
    }

    internal IncomingPacket GetPacket(int index)
    {
        var frame = _frames[index];
        var source = frame.FromArena ? _arena : _buffer;
        return new IncomingPacket(frame.Id, source.AsMemory(frame.Offset, frame.Length));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // a read abandoned mid-flight may still be pinned by the operating system: drop those
        // buffers instead of handing them back to a pool that would lend them out again
        var abandon = Volatile.Read(ref _reading) == 1;

        _count = 0;
        var buffer = _buffer;
        _buffer = [];
        if (!abandon && buffer.Length > 0) ArrayPool<byte>.Shared.Return(buffer);

        var arena = _arena;
        _arena = [];
        if (!abandon && arena.Length > 0) ArrayPool<byte>.Shared.Return(arena);
    }

    private readonly struct Frame
    {
        public readonly int Id;
        public readonly int Offset;
        public readonly int Length;
        public readonly bool FromArena;

        public Frame(int id, int offset, int length, bool fromArena)
        {
            Id = id;
            Offset = offset;
            Length = length;
            FromArena = fromArena;
        }
    }
}
