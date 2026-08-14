using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks.Sources;
using McProtoNet.Cryptography;

namespace McProtoNet.Net;

public sealed class CryptoPipeReader : PipeReader, IDisposable
{
    private const int SegmentSize = 4096;

    private readonly PipeReader _inner;

    private PacketCipher? _decryptor;
    private bool _isEncrypted;
    private bool _innerCompleted;
    private bool _cancelPending;
    private bool _disposed;
    private bool _plainReadOutstanding;
    private int _readState;

    private BufferSegment? _head;
    private BufferSegment? _tail;
    private int _headConsumed;
    private BufferSegment? _examinedSegment;
    private int _examinedIndex;
    private byte[]? _spare;

    public CryptoPipeReader(PipeReader baseReader)
    {
        ArgumentNullException.ThrowIfNull(baseReader);
        _inner = baseReader;
    }

    public bool EncryptionEnabled => _isEncrypted;

    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        PacketCipher decryptor = PacketCipher.CreateDecryptor(sharedSecret);
        try
        {
            EnableEncryption(decryptor);
        }
        catch
        {
            decryptor.Dispose();
            throw;
        }
    }

    public void EnableEncryption(PacketCipher decryptor)
    {
        ArgumentNullException.ThrowIfNull(decryptor);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_isEncrypted)
        {
            throw new InvalidOperationException("Encryption is already enabled.");
        }

        if (_plainReadOutstanding)
        {
            throw new InvalidOperationException(
                "Encryption must be enabled at a read boundary: advance the outstanding plaintext read first.");
        }

        _decryptor = decryptor;
        _isEncrypted = true;
    }

    public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (!_isEncrypted)
        {
            _plainReadOutstanding = true;
            return _inner.ReadAsync(cancellationToken);
        }

        if (Interlocked.CompareExchange(ref _readState, 1, 0) != 0)
        {
            return ValueTask.FromException<ReadResult>(
                new InvalidOperationException("Concurrent reads or writes are not supported."));
        }

        bool release = true;
        try
        {
            if (_cancelPending)
            {
                _cancelPending = false;
                return new ValueTask<ReadResult>(
                    new ReadResult(CurrentSequence(), isCanceled: true, isCompleted: _innerCompleted));
            }

            if (HasUnexaminedBytes())
            {
                return new ValueTask<ReadResult>(
                    new ReadResult(CurrentSequence(), isCanceled: false, isCompleted: _innerCompleted));
            }

            if (_innerCompleted)
            {
                return new ValueTask<ReadResult>(
                    new ReadResult(CurrentSequence(), isCanceled: false, isCompleted: true));
            }

            release = false;
            var source = new PendingReadSource();
            _ = PumpAsync(source, cancellationToken);
            return new ValueTask<ReadResult>(source, 0);
        }
        finally
        {
            if (release)
            {
                Volatile.Write(ref _readState, 0);
            }
        }
    }

    public override bool TryRead(out ReadResult result)
    {
        if (!_isEncrypted)
        {
            bool hasResult = _inner.TryRead(out result);
            if (hasResult)
            {
                _plainReadOutstanding = true;
            }

            return hasResult;
        }

        if (_cancelPending)
        {
            _cancelPending = false;
            result = new ReadResult(CurrentSequence(), isCanceled: true, isCompleted: _innerCompleted);
            return true;
        }

        if (HasUnexaminedBytes())
        {
            result = new ReadResult(CurrentSequence(), isCanceled: false, isCompleted: _innerCompleted);
            return true;
        }

        if (!_innerCompleted && _inner.TryRead(out ReadResult innerResult))
        {
            if (innerResult.Buffer.Length > 0)
            {
                Append(in innerResult);
            }

            _inner.AdvanceTo(innerResult.Buffer.End);

            if (innerResult.IsCompleted)
            {
                _innerCompleted = true;
            }

            if (innerResult.Buffer.Length > 0 || _innerCompleted)
            {
                result = new ReadResult(CurrentSequence(), innerResult.IsCanceled, _innerCompleted);
                return true;
            }
        }

        if (_innerCompleted)
        {
            result = new ReadResult(CurrentSequence(), isCanceled: false, isCompleted: true);
            return true;
        }

        result = default;
        return false;
    }

    public override void AdvanceTo(SequencePosition consumed)
    {
        AdvanceTo(consumed, consumed);
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        if (!_isEncrypted)
        {
            _plainReadOutstanding = false;
            _inner.AdvanceTo(consumed, examined);
            return;
        }

        if (examined.GetObject() is BufferSegment examinedSegment)
        {
            _examinedSegment = examinedSegment;
            _examinedIndex = examined.GetInteger();
        }

        ReleaseConsumed(consumed);
    }

    public override void CancelPendingRead()
    {
        if (!_isEncrypted)
        {
            _inner.CancelPendingRead();
            return;
        }

        _cancelPending = true;
        if (Volatile.Read(ref _readState) != 0)
        {
            _inner.CancelPendingRead();
        }
    }

    public override void Complete(Exception? exception = null)
    {
        ThrowIfReading();
        ReleaseAll();
        _inner.Complete(exception);
    }

    public override ValueTask CompleteAsync(Exception? exception = null)
    {
        ThrowIfReading();
        ReleaseAll();
        return _inner.CompleteAsync(exception);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        ThrowIfReading();
        _disposed = true;
        ReleaseAll();

        if (_spare is not null)
        {
            ArrayPool<byte>.Shared.Return(_spare);
            _spare = null;
        }

        Interlocked.Exchange(ref _decryptor, null)?.Dispose();
    }

    private async Task PumpAsync(PendingReadSource source, CancellationToken cancellationToken)
    {
        ReadResult completion;
        try
        {
            while (true)
            {
                ReadResult result = await _inner.ReadAsync(cancellationToken).ConfigureAwait(false);

                if (result.Buffer.Length > 0)
                {
                    Append(in result);
                }

                _inner.AdvanceTo(result.Buffer.End);

                if (result.IsCompleted)
                {
                    _innerCompleted = true;
                }

                if (result.IsCanceled || _cancelPending)
                {
                    _cancelPending = false;
                    completion = new ReadResult(CurrentSequence(), isCanceled: true, isCompleted: _innerCompleted);
                    break;
                }

                if (result.Buffer.Length > 0 || _innerCompleted)
                {
                    completion = new ReadResult(CurrentSequence(), isCanceled: false, isCompleted: _innerCompleted);
                    break;
                }
            }
        }
        catch (Exception exception)
        {
            Volatile.Write(ref _readState, 0);
            source.Fault(exception);
            return;
        }

        Volatile.Write(ref _readState, 0);
        source.Complete(completion);
    }

    private void Append(in ReadResult innerResult)
    {
        PacketCipher decryptor = _decryptor
            ?? throw new InvalidOperationException("Decryption is not initialized.");

        foreach (ReadOnlyMemory<byte> segment in innerResult.Buffer)
        {
            ReadOnlySpan<byte> source = segment.Span;
            while (!source.IsEmpty)
            {
                BufferSegment tail = AcquireTail();
                int count = Math.Min(source.Length, tail.Buffer.Length - tail.Written);
                Span<byte> destination = tail.Buffer.AsSpan(tail.Written, count);
                source[..count].CopyTo(destination);
                decryptor.Transform(destination);
                tail.Written += count;
                tail.UpdateMemory();
                source = source[count..];
            }
        }
    }

    private BufferSegment AcquireTail()
    {
        if (_tail is not null && _tail.Written < _tail.Buffer.Length)
        {
            return _tail;
        }

        byte[] buffer;
        if (_spare is not null)
        {
            buffer = _spare;
            _spare = null;
        }
        else
        {
            buffer = ArrayPool<byte>.Shared.Rent(SegmentSize);
        }

        var segment = new BufferSegment(buffer);
        if (_tail is null)
        {
            _head = segment;
            _tail = segment;
            _headConsumed = 0;
        }
        else
        {
            _tail.Append(segment);
            _tail = segment;
        }

        return segment;
    }

    private ReadOnlySequence<byte> CurrentSequence()
    {
        if (_head is null || _tail is null)
        {
            return ReadOnlySequence<byte>.Empty;
        }

        return new ReadOnlySequence<byte>(_head, _headConsumed, _tail, _tail.Written);
    }

    private bool HasUnexaminedBytes()
    {
        if (_tail is null || _head is null)
        {
            return false;
        }

        if (_examinedSegment is null)
        {
            return !(ReferenceEquals(_head, _tail) && _headConsumed == _tail.Written);
        }

        return !(ReferenceEquals(_examinedSegment, _tail) && _examinedIndex == _tail.Written);
    }

    private void ReleaseConsumed(SequencePosition consumed)
    {
        if (consumed.GetObject() is not BufferSegment target)
        {
            return;
        }

        bool found = false;
        for (BufferSegment? segment = _head; segment is not null; segment = (BufferSegment?)segment.Next)
        {
            if (ReferenceEquals(segment, target))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            throw new InvalidOperationException("The consumed position does not belong to this reader.");
        }

        while (_head is not null && !ReferenceEquals(_head, target))
        {
            ReleaseHead();
            _headConsumed = 0;
        }

        if (_head is null)
        {
            return;
        }

        _headConsumed = consumed.GetInteger();

        while (_head is not null && _headConsumed == _head.Written)
        {
            if (ReferenceEquals(_head, _tail))
            {
                ReleaseHead();
                _tail = null;
                _headConsumed = 0;
                _examinedSegment = null;
                _examinedIndex = 0;
                return;
            }

            ReleaseHead();
            _headConsumed = 0;
        }
    }

    private void ReleaseHead()
    {
        BufferSegment head = _head!;
        _head = (BufferSegment?)head.Next;
        Recycle(head.Buffer);
    }

    private void ReleaseAll()
    {
        while (_head is not null)
        {
            ReleaseHead();
        }

        _tail = null;
        _headConsumed = 0;
        _examinedSegment = null;
        _examinedIndex = 0;
    }

    private void ThrowIfReading()
    {
        if (Volatile.Read(ref _readState) != 0)
        {
            throw new InvalidOperationException("Cannot complete or dispose the reader while a read is in progress.");
        }
    }

    private void Recycle(byte[] buffer)
    {
        if (_spare is null && buffer.Length == SegmentSize)
        {
            _spare = buffer;
            return;
        }

        ArrayPool<byte>.Shared.Return(buffer);
    }

    private sealed class PendingReadSource : IValueTaskSource<ReadResult>
    {
        private static readonly object CompletedSentinel = new();

        private object? _registration;
        private int _completed;
        private volatile bool _resultAvailable;
        private ReadResult _result;
        private Exception? _exception;

        public void Complete(ReadResult result)
        {
            if (Interlocked.CompareExchange(ref _completed, 1, 0) != 0)
            {
                return;
            }

            _result = result;
            _resultAvailable = true;
            Publish();
        }

        public void Fault(Exception exception)
        {
            if (Interlocked.CompareExchange(ref _completed, 1, 0) != 0)
            {
                return;
            }

            _exception = exception;
            _resultAvailable = true;
            Publish();
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            if (!_resultAvailable)
            {
                return ValueTaskSourceStatus.Pending;
            }

            return _exception switch
            {
                null => ValueTaskSourceStatus.Succeeded,
                OperationCanceledException => ValueTaskSourceStatus.Canceled,
                _ => ValueTaskSourceStatus.Faulted
            };
        }

        public ReadResult GetResult(short token)
        {
            if (!_resultAvailable)
            {
                throw new InvalidOperationException("The read has not completed yet.");
            }

            if (_exception is not null)
            {
                ExceptionDispatchInfo.Capture(_exception).Throw();
            }

            return _result;
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token,
            ValueTaskSourceOnCompletedFlags flags)
        {
            var holder = new ContinuationHolder(continuation, state);

            while (true)
            {
                object? current = Volatile.Read(ref _registration);

                if (ReferenceEquals(current, CompletedSentinel))
                {
                    Schedule(holder);
                    return;
                }

                if (current is null)
                {
                    if (Interlocked.CompareExchange(ref _registration, holder, null) is null)
                    {
                        return;
                    }

                    continue;
                }

                if (Interlocked.CompareExchange(ref _completed, 1, 0) == 0)
                {
                    _exception = new InvalidOperationException("Concurrent reads or writes are not supported.");
                    _resultAvailable = true;
                }

                object? first = Interlocked.Exchange(ref _registration, CompletedSentinel);
                if (first is ContinuationHolder firstHolder)
                {
                    Schedule(firstHolder);
                }

                Schedule(holder);
                return;
            }
        }

        private void Publish()
        {
            object? registration = Interlocked.Exchange(ref _registration, CompletedSentinel);
            if (registration is ContinuationHolder holder)
            {
                Schedule(holder);
            }
        }

        private static void Schedule(ContinuationHolder holder)
        {
            ThreadPool.UnsafeQueueUserWorkItem(
                static h => h.Continuation(h.State),
                holder,
                preferLocal: true);
        }

        private sealed record ContinuationHolder(Action<object?> Continuation, object? State);
    }

    private sealed class BufferSegment : ReadOnlySequenceSegment<byte>
    {
        public BufferSegment(byte[] buffer)
        {
            Buffer = buffer;
            Memory = ReadOnlyMemory<byte>.Empty;
        }

        public byte[] Buffer { get; }

        public int Written { get; set; }

        public void UpdateMemory()
        {
            Memory = new ReadOnlyMemory<byte>(Buffer, 0, Written);
        }

        public void Append(BufferSegment next)
        {
            next.RunningIndex = RunningIndex + Written;
            Next = next;
        }
    }
}
