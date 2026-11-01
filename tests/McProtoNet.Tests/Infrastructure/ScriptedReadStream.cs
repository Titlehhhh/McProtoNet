namespace McProtoNet.Tests.Infrastructure;

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
