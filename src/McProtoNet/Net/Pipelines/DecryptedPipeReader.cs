using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using DotNext.Threading;
using Org.BouncyCastle.Crypto;


public sealed class DecryptedPipeReader : PipeReader
{
    private readonly PipeReader _pipeReader;
    private readonly Pipe _encPipe;

    private IBufferedCipher? _decryptor;
    private bool _isEncrypted;

    public bool IsEncrypted => _isEncrypted;

    private int _readState = 0;

    public DecryptedPipeReader(PipeReader baseReader, Pipe? pipe = null)
    {
        _pipeReader = baseReader ?? throw new ArgumentNullException(nameof(baseReader));
        _encPipe = pipe ?? new Pipe(new PipeOptions(
            readerScheduler: PipeScheduler.Inline,
            writerScheduler: PipeScheduler.Inline));
    }

    public void SwitchEncryption(IBufferedCipher cipher)
    {
        _decryptor = cipher ?? throw new ArgumentNullException(nameof(cipher));
        _isEncrypted = true;
    }

    public override bool TryRead(out ReadResult result)
    {
        if (_isEncrypted)
        {
            if (_encPipe.Reader.TryRead(out result))
                return true;


            if (_pipeReader.TryRead(out var r1))
            {
                if (r1.Buffer.Length > 0)
                {
                    Decrypt(r1.Buffer, _encPipe.Writer);
                    _pipeReader.AdvanceTo(r1.Buffer.End);
                    ValueTask<FlushResult> flushAsync = _encPipe.Writer.FlushAsync();
                    Debug.Assert(!flushAsync.IsCompletedSuccessfully);
                }

                if (_encPipe.Reader.TryRead(out result))
                    return true;

                if (r1.IsCompleted)
                {
                    _encPipe.Writer.Complete();
                }
            }

            result = default;
            return false;
        }

        return _pipeReader.TryRead(out result);
    }

    public override ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = default)
    {
        if (!_isEncrypted)
            return _pipeReader.ReadAsync(cancellationToken);

        if (Interlocked.CompareExchange(ref _readState, 1, 0) != 0)
        {
            return
                ValueTask.FromException<ReadResult>
                    (new InvalidOperationException("Concurrent reads or writes are not supported."));
        }

        return new ValueTask<ReadResult>(ReadDecryptedAsync(cancellationToken));
    }

    private async Task<ReadResult> ReadDecryptedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var readResult = await _pipeReader.ReadAsync(cancellationToken);
            if (readResult.Buffer.Length > 0)
            {
                Decrypt(readResult.Buffer, _encPipe.Writer);
                _pipeReader.AdvanceTo(readResult.Buffer.End);
                FlushResult result = await _encPipe.Writer.FlushAsync(cancellationToken);
                if (result.IsCompleted)
                {
                    await _encPipe.Writer.CompleteAsync();
                }
                //Debug.Assert(!result.IsCompletedSuccessfully);
            }

            if (readResult.IsCompleted)
            {
                await _encPipe.Writer.CompleteAsync();
            }

            return await _encPipe.Reader.ReadAsync(cancellationToken);
        }
        finally
        {
            Interlocked.Exchange(ref _readState, 0);
        }
    }

    private void Decrypt(in ReadOnlySequence<byte> data, PipeWriter output)
    {
        if (!_isEncrypted || _decryptor is null)
            throw new InvalidOperationException("Decryption is not initialized.");

        foreach (ReadOnlyMemory<byte> segment in data)
        {
            ReadOnlySpan<byte> src = segment.Span;
            int outSize = src.Length;
            Span<byte> dest = output.GetSpan(outSize);

            int written = _decryptor.ProcessBytes(src, dest);
            Debug.Assert(written <= dest.Length, "ProcessBytes wrote more than allocated buffer");

            if (written > 0)
                output.Advance(written);
        }
    }

    public override void AdvanceTo(SequencePosition consumed)
    {
        if (_isEncrypted)
            _encPipe.Reader.AdvanceTo(consumed);
        else
            _pipeReader.AdvanceTo(consumed);
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        if (_isEncrypted)
            _encPipe.Reader.AdvanceTo(consumed, examined);
        else
            _pipeReader.AdvanceTo(consumed, examined);
    }

    public override void CancelPendingRead()
    {
        if (_isEncrypted)
        {
            _encPipe.Reader.CancelPendingRead();
        }

        _pipeReader.CancelPendingRead();
    }

    public override void Complete(Exception? exception = null)
    {
        if (_isEncrypted)
        {
            _encPipe.Reader.Complete(exception);
            _encPipe.Writer.Complete(exception);
        }

        _pipeReader.Complete(exception);
    }
}