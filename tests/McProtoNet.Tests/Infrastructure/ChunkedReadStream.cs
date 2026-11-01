namespace McProtoNet.Tests.Infrastructure;

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
