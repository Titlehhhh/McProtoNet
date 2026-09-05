using System.Buffers;
using System.Runtime.CompilerServices;
using McProtoNet.Primitives;
using McProtoNet.Transport.Cryptography;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Provides a reader that reads frames in batches from one pooled block, decrypting in place and
/// inflating into an arena block.
/// </summary>
/// <remarks>
/// The reader holds one reference to its read block and one to its arena; the batch enumerator holds
/// one to the block behind the packet it stands on. A block is overwritten in place only
/// while the reader is its sole holder; otherwise the reader moves on to a fresh block and the old one
/// dies with its last packet. Only the reading thread touches the blocks: a Dispose from another
/// thread during a read is carried out by that read when it ends.
/// </remarks>
internal sealed class BufferedPacketReader : IDisposable
{
    private const int DefaultCapacity = 64 * 1024;

    /// <summary>The largest frame length accepted from the wire, in bytes.</summary>
    public const int MaxFrameLength = 32 * 1024 * 1024;

    private readonly Stream _stream;
    private readonly ArrayPool<byte> _pool;
    private readonly PacketCipher? _cipher;
    private readonly int _compressionThreshold;

    private PooledBlock _block;
    private int _start;
    private int _end;

    private PooledBlock? _arena;
    private int _arenaUsed;

    // arenas outgrown inside the current batch: frames of the batch still point into them, so the
    // reader lets go of them only when the next batch starts
    private readonly List<PooledBlock> _retired = [];

    private Frame[] _frames = new Frame[64];
    private int _count;
    private int _needed = 1;

    private const int Idle = 0;
    private const int Reading = 1;
    private const int Released = 2;

    private int _reading;
    private volatile bool _disposed;
    private bool _eof;

    public BufferedPacketReader(Stream stream, int compressionThreshold = -1, PacketCipher? cipher = null,
        int initialCapacity = DefaultCapacity, ArrayPool<byte>? pool = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
        _pool = pool ?? ArrayPool<byte>.Shared;
        _compressionThreshold = compressionThreshold;
        _cipher = cipher;
        _block = new PooledBlock(_pool, Math.Max(initialCapacity, 1024));
    }

    /// <summary>Gets the compression threshold, in bytes. A negative value disables compression.</summary>
    public int CompressionThreshold => _compressionThreshold;

    /// <summary>Gets a value indicating whether a decryptor is attached.</summary>
    public bool IsEncrypted => _cipher is not null;

    /// <summary>Gets the number of bytes already read but not yet handed out as packets.</summary>
    public int BufferedBytes => _end - _start;

    /// <summary>Asynchronously reads until at least one whole frame is available and returns all of them.</summary>
    public ValueTask<PacketBatch> ReadBatchAsync(CancellationToken token = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var state = Interlocked.CompareExchange(ref _reading, Reading, Idle);
        if (state == Reading) ThrowHelper.ThrowConcurrentRead();
        ObjectDisposedException.ThrowIf(state == Released, this);

        try
        {
            token.ThrowIfCancellationRequested();

            StartBatch();

            ParseFrames();
            if (_count > 0)
            {
                EndRead();
                return new ValueTask<PacketBatch>(new PacketBatch(this, _count, false));
            }

            if (_eof)
            {
                if (_end > _start) ThrowHelper.ThrowTruncatedFrame();

                EndRead();
                return new ValueTask<PacketBatch>(new PacketBatch(this, 0, true));
            }
        }
        catch
        {
            EndRead();
            throw;
        }

        return FillAndParseAsync(token);
    }

    /// <summary>Asynchronously reads batches and returns their packets as one sequence that ends at end of stream.</summary>
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

                var read = await _stream.ReadAsync(_block.Array.AsMemory(_end), token).ConfigureAwait(false);
                if (read == 0)
                {
                    _eof = true;
                    if (_end > _start) ThrowHelper.ThrowTruncatedFrame();

                    return new PacketBatch(this, 0, true);
                }

                _cipher?.Transform(_block.Array.AsSpan(_end, read));
                _end += read;

                ParseFrames();
                if (_count > 0) return new PacketBatch(this, _count, false);
            }
        }
        finally
        {
            EndRead();
        }
    }

    /// <summary>
    /// Ends the read. A Dispose that landed meanwhile left the blocks alone, because the read still
    /// owned them; it is carried out here, once the read let go.
    /// </summary>
    private void EndRead()
    {
        Volatile.Write(ref _reading, Idle);
        if (_disposed && Interlocked.CompareExchange(ref _reading, Released, Idle) == Idle) ReleaseAll();
    }

    /// <summary>
    /// Forgets the frames of the previous batch: they left with references of their own. The arena is
    /// reused from the top only while nobody else holds it.
    /// </summary>
    private void StartBatch()
    {
        ReleaseRetired();
        if (_arena is { IsShared: true })
        {
            _arena.Release();
            _arena = null;
        }

        _arenaUsed = 0;
        _count = 0;
    }

    private void ReleaseRetired()
    {
        foreach (var block in _retired) block.Release();
        _retired.Clear();
    }

    private void MakeRoom()
    {
        // room at the tail to read into, and the frame being assembled still fits where it lies
        var capacity = _block.Length;
        if (_end < capacity && capacity - _start >= _needed) return;

        // the tail slides down in place only while the reader is the sole holder: packets still out
        // point below _start, and a copy over them would corrupt what they see
        if (_start > 0 && !_block.IsShared)
        {
            if (_end > _start) _block.Array.AsSpan(_start, _end - _start).CopyTo(_block.Array);
            _end -= _start;
            _start = 0;
        }

        if (_start > 0) Move(Math.Max(_needed, capacity));
        else if (capacity < _needed) Move(_needed);
        else if (capacity == _end) Move(capacity * 2);
    }

    /// <summary>Carries the tail into a fresh block; the old one stays with whoever still holds it.</summary>
    private void Move(int minimumLength)
    {
        var next = new PooledBlock(_pool, minimumLength);
        _block.Array.AsSpan(_start, _end - _start).CopyTo(next.Array);
        _block.Release();
        _block = next;
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

            if (!_block.Array.AsSpan(_start, available).TryReadVarInt(out var length, out var lengthBytes))
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
        var array = _block.Array;
        if (_compressionThreshold < 0)
        {
            var id = array.AsSpan(offset, length).ReadVarInt(out var idLength);
            Append(id, _block, offset + idLength, length - idLength);
            return;
        }

        var sizeUncompressed = array.AsSpan(offset, length).ReadVarInt(out var sizeLength);
        if (sizeUncompressed <= 0)
        {
            var plainLength = length - sizeLength;
            if (plainLength <= 0) ThrowHelper.ThrowEmptyEnvelope();

            var id = array.AsSpan(offset + sizeLength, plainLength).ReadVarInt(out var idLength);
            Append(id, _block, offset + sizeLength + idLength, plainLength - idLength);
            return;
        }

        if (sizeUncompressed > MaxFrameLength || length - sizeLength <= 0)
            ThrowHelper.ThrowInvalidUncompressedSize(sizeUncompressed);

        var arena = ArenaAllocate(sizeUncompressed, out var target);
        PacketStreamReader.DecompressCore(
            array.AsSpan(offset + sizeLength, length - sizeLength),
            arena.Array.AsSpan(target, sizeUncompressed));

        var packetId = arena.Array.AsSpan(target, sizeUncompressed).ReadVarInt(out var packetIdLength);
        Append(packetId, arena, target + packetIdLength, sizeUncompressed - packetIdLength);
    }

    /// <summary>
    /// Finds room for one inflated frame. An arena that runs out is not copied: it stays with the
    /// frames already in it, and a fresh one takes the next frames.
    /// </summary>
    private PooledBlock ArenaAllocate(int size, out int offset)
    {
        if (_arena is null || _arena.Length - _arenaUsed < size)
        {
            var wanted = Math.Max(size, Math.Max((_arena?.Length ?? 0) * 2, DefaultCapacity));
            if (_arena is not null)
            {
                if (_arenaUsed > 0) _retired.Add(_arena);
                else _arena.Release();
            }

            _arena = new PooledBlock(_pool, wanted);
            _arenaUsed = 0;
        }

        offset = _arenaUsed;
        _arenaUsed += size;
        return _arena;
    }

    private void Append(int id, PooledBlock block, int offset, int length)
    {
        if (length < 0) ThrowHelper.ThrowIdPastFrameEnd();
        if (_count == _frames.Length) Array.Resize(ref _frames, _frames.Length * 2);
        _frames[_count++] = new Frame(id, block, offset, length);
    }

    /// <summary>Hands out the packet at the index, owning one reference to the block behind it.</summary>
    internal IncomingPacket GetPacket(int index)
    {
        var frame = _frames[index];
        frame.Block.Retain();
        return new IncomingPacket(frame.Id, frame.Block, frame.Offset, frame.Length);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // a read in flight still owns the blocks, and the operating system may still be writing into
        // one of them: that read releases everything when it ends, in EndRead
        if (Interlocked.CompareExchange(ref _reading, Released, Idle) == Idle) ReleaseAll();
    }

    private void ReleaseAll()
    {
        _count = 0;
        _block.Release();
        _arena?.Release();
        _arena = null;
        ReleaseRetired();
    }

    private readonly struct Frame
    {
        public readonly int Id;
        public readonly PooledBlock Block;
        public readonly int Offset;
        public readonly int Length;

        public Frame(int id, PooledBlock block, int offset, int length)
        {
            Id = id;
            Block = block;
            Offset = offset;
            Length = length;
        }
    }
}
