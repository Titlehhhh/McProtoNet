using System.Buffers;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Infrastructure;

public sealed record TestPacket(int Id, byte[] Body);

/// <summary>Builds wire bytes the same way the transport does, so readers can be compared to them.</summary>
public static class Frames
{
    /// <summary>Body sizes that hit every varint width, both sides of a 256 threshold, and 70 KiB.</summary>
    private static readonly int[] Sizes =
    [
        0, 1, 2, 3, 5, 15, 63, 126, 127, 128, 129, 254, 255, 256, 257, 300, 511, 512, 1000, 1024,
        4095, 4096, 8192, 16 * 1024, 32 * 1024, 64 * 1024, 65535, 65536, 70 * 1024
    ];

    /// <summary>A cheap sample for byte-at-a-time runs: same varint widths, nothing huge.</summary>
    public static List<TestPacket> SmallSample(int seed)
    {
        var random = new Random(seed);
        int[] sizes = [0, 1, 2, 5, 63, 126, 127, 128, 129, 255, 256, 257, 300, 1000];
        var packets = new List<TestPacket>();
        foreach (var size in sizes)
        {
            var body = new byte[size];
            random.NextBytes(body);
            packets.Add(new TestPacket(random.Next(0, 1 << 20), body));
        }

        return packets;
    }

    public static List<TestPacket> Sample(int seed, int repeats = 3)
    {
        var random = new Random(seed);
        var packets = new List<TestPacket>();

        for (var round = 0; round < repeats; round++)
        {
            foreach (var size in Sizes)
            {
                var body = new byte[size];
                // half random (incompressible), half repetitive (compresses hard) — both paths get exercised
                if (round % 2 == 0) random.NextBytes(body);
                else for (var i = 0; i < body.Length; i++) body[i] = (byte)(i % 7);

                packets.Add(new TestPacket(random.Next(0, 1 << 20), body));
            }
        }

        return packets;
    }

    public static byte[] Build(IReadOnlyList<TestPacket> packets, int compressionThreshold, bool encrypted)
    {
        var writer = new ArrayBufferWriter<byte>(1 << 20);
        foreach (var packet in packets)
            writer.WritePacket(packet.Id, packet.Body, compressionThreshold);

        var wire = writer.WrittenSpan.ToArray();
        if (encrypted)
        {
            using var encryptor = Crypto.CreateEncryptor();
            encryptor.Transform(wire);
        }

        return wire;
    }

    /// <summary>Frame end offsets in the wire, so a reader's stream position can be checked exactly.</summary>
    public static int[] FrameEnds(IReadOnlyList<TestPacket> packets, int compressionThreshold)
    {
        var ends = new int[packets.Count];
        var writer = new ArrayBufferWriter<byte>(1 << 20);
        for (var i = 0; i < packets.Count; i++)
        {
            writer.WritePacket(packets[i].Id, packets[i].Body, compressionThreshold);
            ends[i] = writer.WrittenCount;
        }

        return ends;
    }

    public static PacketCipher? Decryptor(bool encrypted) => encrypted ? Crypto.CreateDecryptor() : null;
}

/// <summary>A read-only stream that hands out random slices, so framing never sees tidy boundaries.</summary>
public sealed class ChunkedReadStream : Stream
{
    private readonly byte[] _data;
    private readonly Random _random;
    private readonly int _maxChunk;
    private int _position;

    public ChunkedReadStream(byte[] data, int seed, int maxChunk = 64 * 1024)
    {
        _data = data;
        _random = new Random(seed);
        _maxChunk = maxChunk;
    }

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _data.Length;

    public override long Position
    {
        get => _position;
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

    public override int Read(Span<byte> buffer)
    {
        var left = _data.Length - _position;
        if (left == 0) return 0;

        var take = Math.Min(Math.Min(buffer.Length, left), 1 + _random.Next(_maxChunk));
        _data.AsSpan(_position, take).CopyTo(buffer);
        _position += take;
        return take;
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(Read(buffer.Span));
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

/// <summary>
///     A stream whose reads and writes park until released — the shape needed to abort or cancel an
///     operation that is genuinely in flight. Disposing it fails the parked operations, like a socket.
/// </summary>
public sealed class GateStream : Stream
{
    private readonly TaskCompletionSource<int> _read = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _write = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _readStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _writeStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _disposed;

    public Task ReadStarted => _readStarted.Task;
    public Task WriteStarted => _writeStarted.Task;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public void ReleaseRead(int bytes) => _read.TrySetResult(bytes);

    public void ReleaseWrite() => _write.TrySetResult();

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _readStarted.TrySetResult();
        return await _read.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writeStarted.TrySetResult();
        await _write.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected override void Dispose(bool disposing)
    {
        _disposed = true;
        _read.TrySetException(new ObjectDisposedException(nameof(GateStream)));
        _write.TrySetException(new ObjectDisposedException(nameof(GateStream)));
        base.Dispose(disposing);
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}

/// <summary>
///     Reads hand out one scripted chunk at a time and park when the script runs dry, so a read can be
///     cancelled mid-frame and resumed later without losing a byte.
/// </summary>
public sealed class ScriptedReadStream : Stream
{
    private readonly List<byte[]> _chunks = [];
    private TaskCompletionSource _pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _offset;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public void Push(ReadOnlySpan<byte> chunk)
    {
        lock (_chunks)
        {
            _chunks.Add(chunk.ToArray());
            var waiting = _pending;
            _pending = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            waiting.TrySetResult();
        }
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            Task waiting;
            lock (_chunks)
            {
                if (_chunks.Count > 0)
                {
                    var chunk = _chunks[0];
                    var take = Math.Min(buffer.Length, chunk.Length - _offset);
                    chunk.AsSpan(_offset, take).CopyTo(buffer.Span);
                    _offset += take;
                    if (_offset == chunk.Length)
                    {
                        _chunks.RemoveAt(0);
                        _offset = 0;
                    }

                    return take;
                }

                waiting = _pending.Task;
            }

            await waiting.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}

/// <summary>
///     A stream whose reads and writes always fail with one chosen exception — for the rules about
///     what a connection does with a peer failure and with a cancellation that is not the caller's.
/// </summary>
public sealed class FailingStream : Stream
{
    private readonly Exception _error;

    public FailingStream(Exception error) => _error = error;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => ValueTask.FromException<int>(_error);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => ValueTask.FromException(_error);

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}

/// <summary>A stream whose writes always fail — for the "a broken flush kills the writer" rule.</summary>
public sealed class FailingWriteStream : Stream
{
    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => ValueTask.FromException(new IOException("write failed"));

    public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override void Flush() => throw new NotSupportedException();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}
