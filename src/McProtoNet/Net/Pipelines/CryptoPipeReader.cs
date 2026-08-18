using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks.Sources;
using McProtoNet.Cryptography;

namespace McProtoNet.Net;

public sealed class CryptoPipeReader : PipeReader, IDisposable
{
    private const int MinimumBlockSize = 4096;
    private const int RetainedBlockLimit = 64 * 1024;

    private readonly PipeReader _inner;
    private readonly ReadCompletion _completion = new();
    private readonly Action _onInnerRead;

    private PacketCipher? _decryptor;
    private bool _disposed;
    private bool _plainReadOutstanding;

    private int _reading;
    private int _cancelRequested;
    private bool _resultOutstanding;
    private bool _innerCompleted;
    private CancellationToken _pendingToken;
    private ConfiguredValueTaskAwaitable<ReadResult>.ConfiguredValueTaskAwaiter _innerAwaiter;

    private byte[]? _array;
    private Block? _block;
    private int _consumed;
    private int _examined;
    private int _written;

    public CryptoPipeReader(PipeReader baseReader)
    {
        ArgumentNullException.ThrowIfNull(baseReader);
        _inner = baseReader;
        _onInnerRead = ContinueRead;
    }

    public bool EncryptionEnabled => _decryptor is not null;

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

        if (_decryptor is not null)
        {
            throw new InvalidOperationException("Encryption is already enabled.");
        }

        if (_plainReadOutstanding)
        {
            throw new InvalidOperationException(
                "Encryption must be enabled at a read boundary: advance the outstanding plaintext read first.");
        }

        _decryptor = decryptor;
    }

    public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (_decryptor is null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _plainReadOutstanding = true;
            return _inner.ReadAsync(cancellationToken);
        }

        BeginRead();
        try
        {
            if (TryReadBuffered(out ReadResult result))
            {
                EndRead();
                return new ValueTask<ReadResult>(result);
            }

            while (true)
            {
                var awaiter = _inner.ReadAsync(cancellationToken).ConfigureAwait(false).GetAwaiter();
                if (!awaiter.IsCompleted)
                {
                    return WaitForInner(awaiter, cancellationToken);
                }

                if (Absorb(awaiter.GetResult(), out result))
                {
                    EndRead();
                    return new ValueTask<ReadResult>(result);
                }
            }
        }
        catch
        {
            AbortRead();
            throw;
        }
    }

    public override bool TryRead(out ReadResult result)
    {
        if (_decryptor is null)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            bool hasResult = _inner.TryRead(out result);
            _plainReadOutstanding |= hasResult;
            return hasResult;
        }

        BeginRead();
        try
        {
            if (TryReadBuffered(out result)
                || (_inner.TryRead(out ReadResult innerResult) && Absorb(in innerResult, out result)))
            {
                EndRead();
                return true;
            }

            AbortRead();
            result = default;
            return false;
        }
        catch
        {
            AbortRead();
            throw;
        }
    }

    public override void AdvanceTo(SequencePosition consumed)
    {
        AdvanceTo(consumed, consumed);
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        if (_decryptor is null)
        {
            _plainReadOutstanding = false;
            _inner.AdvanceTo(consumed, examined);
            return;
        }

        if (Volatile.Read(ref _reading) != 0)
        {
            throw new InvalidOperationException("Cannot advance while a read is pending.");
        }

        if (!_resultOutstanding)
        {
            throw new InvalidOperationException("No reading operation to complete.");
        }

        int consumedIndex = IndexOf(consumed, _consumed);
        int examinedIndex = IndexOf(examined, consumedIndex);

        _resultOutstanding = false;
        _consumed = consumedIndex;
        _examined = examinedIndex;

        if (_consumed == _written && _array is { Length: > RetainedBlockLimit })
        {
            ReleaseBuffer();
        }
    }

    public override void CancelPendingRead()
    {
        if (_decryptor is null)
        {
            _inner.CancelPendingRead();
            return;
        }

        Interlocked.Exchange(ref _cancelRequested, 1);
        if (Volatile.Read(ref _reading) != 0)
        {
            _inner.CancelPendingRead();
        }
    }

    public override void Complete(Exception? exception = null)
    {
        Close();
        _inner.Complete(exception);
    }

    public override ValueTask CompleteAsync(Exception? exception = null)
    {
        Close();
        return _inner.CompleteAsync(exception);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Close();
        _disposed = true;
        _decryptor?.Dispose();
    }

    private void Close()
    {
        ThrowIfReading();
        _resultOutstanding = false;
        _plainReadOutstanding = false;
        ReleaseBuffer();
    }

    private void BeginRead()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_resultOutstanding)
        {
            throw new InvalidOperationException("Reading is already in progress.");
        }

        if (Interlocked.CompareExchange(ref _reading, 1, 0) != 0)
        {
            throw new InvalidOperationException("Concurrent reads or writes are not supported.");
        }
    }

    private void EndRead()
    {
        _resultOutstanding = true;
        Volatile.Write(ref _reading, 0);
    }

    private void AbortRead()
    {
        Volatile.Write(ref _reading, 0);
    }

    private ValueTask<ReadResult> WaitForInner(
        ConfiguredValueTaskAwaitable<ReadResult>.ConfiguredValueTaskAwaiter awaiter,
        CancellationToken cancellationToken)
    {
        _pendingToken = cancellationToken;
        _innerAwaiter = awaiter;
        _completion.Reset();
        short version = _completion.Version;
        awaiter.UnsafeOnCompleted(_onInnerRead);
        return new ValueTask<ReadResult>(_completion, version);
    }

    private void ContinueRead()
    {
        ReadResult result;
        try
        {
            ReadResult innerResult = _innerAwaiter.GetResult();
            while (!Absorb(in innerResult, out result))
            {
                var awaiter = _inner.ReadAsync(_pendingToken).ConfigureAwait(false).GetAwaiter();
                if (!awaiter.IsCompleted)
                {
                    _innerAwaiter = awaiter;
                    awaiter.UnsafeOnCompleted(_onInnerRead);
                    return;
                }

                innerResult = awaiter.GetResult();
            }
        }
        catch (Exception exception)
        {
            AbortRead();
            _completion.SetException(exception);
            return;
        }

        EndRead();
        _completion.SetResult(result);
    }

    private bool TryReadBuffered(out ReadResult result)
    {
        if (TakeCancelRequest())
        {
            result = Snapshot(isCanceled: true);
            return true;
        }

        if (_examined < _written || _innerCompleted)
        {
            result = Snapshot(isCanceled: false);
            return true;
        }

        result = default;
        return false;
    }

    private bool Absorb(in ReadResult innerResult, out ReadResult result)
    {
        long length = innerResult.Buffer.Length;
        if (length > 0)
        {
            Append(innerResult.Buffer);
        }

        _inner.AdvanceTo(innerResult.Buffer.End);
        _innerCompleted |= innerResult.IsCompleted;

        bool canceled = TakeCancelRequest() | innerResult.IsCanceled;
        if (length > 0 || canceled || _innerCompleted)
        {
            result = Snapshot(canceled);
            return true;
        }

        result = default;
        return false;
    }

    private bool TakeCancelRequest()
    {
        return Volatile.Read(ref _cancelRequested) != 0 && Interlocked.Exchange(ref _cancelRequested, 0) != 0;
    }

    private ReadResult Snapshot(bool isCanceled)
    {
        ReadOnlySequence<byte> buffer = _block is null
            ? ReadOnlySequence<byte>.Empty
            : new ReadOnlySequence<byte>(_block, _consumed, _block, _written);
        return new ReadResult(buffer, isCanceled, _innerCompleted);
    }

    private void Append(in ReadOnlySequence<byte> ciphertext)
    {
        int length = checked((int)ciphertext.Length);
        byte[] array = MakeRoom(length);

        Span<byte> destination = array.AsSpan(_written, length);
        ciphertext.CopyTo(destination);
        _decryptor!.Transform(destination);

        _written += length;
        _block ??= new Block();
        _block.SetLength(array, _written);
    }

    private byte[] MakeRoom(int length)
    {
        if (_array is null)
        {
            return _array = Rent(length);
        }

        if (_array.Length - _written >= length)
        {
            return _array;
        }

        int live = _written - _consumed;
        if (live <= _array.Length / 2 && live + length <= _array.Length)
        {
            Array.Copy(_array, _consumed, _array, 0, live);
        }
        else
        {
            byte[] grown = Rent(Math.Max(live + length, 2 * _array.Length));
            _array.AsSpan(_consumed, live).CopyTo(grown);
            ArrayPool<byte>.Shared.Return(_array);
            _array = grown;
        }

        _written = live;
        _examined -= _consumed;
        _consumed = 0;
        _block = null;
        return _array;
    }

    private static byte[] Rent(int length)
    {
        return ArrayPool<byte>.Shared.Rent(Math.Max(length, MinimumBlockSize));
    }

    private int IndexOf(SequencePosition position, int lowerBound)
    {
        if (_block is null)
        {
            return lowerBound;
        }

        int index = position.GetInteger();
        if (!ReferenceEquals(position.GetObject(), _block) || index < lowerBound || index > _written)
        {
            throw new InvalidOperationException("The position does not belong to the current read.");
        }

        return index;
    }

    private void ReleaseBuffer()
    {
        _block = null;
        _consumed = 0;
        _examined = 0;
        _written = 0;

        if (_array is not null)
        {
            ArrayPool<byte>.Shared.Return(_array);
            _array = null;
        }
    }

    private void ThrowIfReading()
    {
        if (Volatile.Read(ref _reading) != 0)
        {
            throw new InvalidOperationException("Cannot complete or dispose the reader while a read is in progress.");
        }
    }

    private sealed class Block : ReadOnlySequenceSegment<byte>
    {
        public void SetLength(byte[] array, int length)
        {
            Memory = new ReadOnlyMemory<byte>(array, 0, length);
        }
    }

    private sealed class ReadCompletion : IValueTaskSource<ReadResult>
    {
        private readonly object _sync = new();
        private Action<object?>? _continuation;
        private object? _continuationState;
        private ExecutionContext? _executionContext;
        private SynchronizationContext? _synchronizationContext;
        private volatile bool _completed;
        private ReadResult _result;
        private Exception? _exception;
        private short _version;

        public short Version => _version;

        public void Reset()
        {
            lock (_sync)
            {
                ClearContinuation();
                _completed = false;
                _result = default;
                _exception = null;
                _version++;
            }
        }

        public void SetResult(ReadResult result)
        {
            Finish(result, null);
        }

        public void SetException(Exception exception)
        {
            Finish(default, exception);
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            ValidateToken(token);
            if (!_completed)
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
            ValidateToken(token);
            if (!_completed)
            {
                throw new InvalidOperationException("The read has not completed yet.");
            }

            if (_exception is not null)
            {
                ExceptionDispatchInfo.Throw(_exception);
            }

            return _result;
        }

        public void OnCompleted(Action<object?> continuation, object? state, short token,
            ValueTaskSourceOnCompletedFlags flags)
        {
            ValidateToken(token);

            ExecutionContext? executionContext = (flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0
                ? ExecutionContext.Capture()
                : null;
            SynchronizationContext? synchronizationContext =
                (flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0
                    ? SynchronizationContext.Current
                    : null;

            Action<object?>? displaced;
            object? displacedState;
            ExecutionContext? displacedExecutionContext;
            SynchronizationContext? displacedSynchronizationContext;
            lock (_sync)
            {
                if (!_completed && _continuation is null)
                {
                    _continuation = continuation;
                    _continuationState = state;
                    _executionContext = executionContext;
                    _synchronizationContext = synchronizationContext;
                    return;
                }

                displaced = _continuation;
                displacedState = _continuationState;
                displacedExecutionContext = _executionContext;
                displacedSynchronizationContext = _synchronizationContext;
                ClearContinuation();

                if (!_completed)
                {
                    _completed = true;
                    _exception = new InvalidOperationException("Concurrent reads or writes are not supported.");
                }
            }

            if (displaced is not null)
            {
                Queue(displaced, displacedState, displacedExecutionContext, displacedSynchronizationContext);
            }

            Queue(continuation, state, executionContext, synchronizationContext);
        }

        private void Finish(ReadResult result, Exception? exception)
        {
            Action<object?>? continuation;
            object? state;
            ExecutionContext? executionContext;
            SynchronizationContext? synchronizationContext;
            lock (_sync)
            {
                if (_completed)
                {
                    return;
                }

                _result = result;
                _exception = exception;
                _completed = true;
                continuation = _continuation;
                state = _continuationState;
                executionContext = _executionContext;
                synchronizationContext = _synchronizationContext;
                ClearContinuation();
            }

            if (continuation is null)
            {
                return;
            }

            if (synchronizationContext is not null)
            {
                Queue(continuation, state, executionContext, synchronizationContext);
            }
            else
            {
                Invoke(continuation, state, executionContext);
            }
        }

        private void ClearContinuation()
        {
            _continuation = null;
            _continuationState = null;
            _executionContext = null;
            _synchronizationContext = null;
        }

        private static void Queue(Action<object?> continuation, object? state,
            ExecutionContext? executionContext, SynchronizationContext? synchronizationContext)
        {
            var work = (continuation, state, executionContext);
            if (synchronizationContext is not null)
            {
                synchronizationContext.Post(static o => RunQueued(o!), work);
            }
            else
            {
                ThreadPool.UnsafeQueueUserWorkItem(static o => RunQueued(o), work, preferLocal: true);
            }
        }

        private static void RunQueued(object work)
        {
            var (continuation, state, executionContext) =
                ((Action<object?>, object?, ExecutionContext?))work;
            Invoke(continuation, state, executionContext);
        }

        private static void Invoke(Action<object?> continuation, object? state, ExecutionContext? executionContext)
        {
            if (executionContext is null)
            {
                continuation(state);
            }
            else
            {
                ExecutionContext.Run(executionContext, static o => RunQueued(o!), (continuation, state, (ExecutionContext?)null));
            }
        }

        private void ValidateToken(short token)
        {
            if (token != _version)
            {
                throw new InvalidOperationException("The read result was already observed.");
            }
        }
    }
}
