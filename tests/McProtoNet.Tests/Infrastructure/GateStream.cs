namespace McProtoNet.Tests.Infrastructure;

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
