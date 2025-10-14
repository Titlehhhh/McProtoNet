using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using Org.BouncyCastle.Crypto;

public sealed class EncryptedPipeWriter : PipeWriter
{
    private readonly PipeWriter _pipeWriter;
    private readonly Pipe _encPipe;

    private bool _isEncrypted;
    private IBufferedCipher? _encryptor;

    public EncryptedPipeWriter(PipeWriter pipeWriter)
    {
        _pipeWriter = pipeWriter;
        _encPipe = new Pipe(new PipeOptions(
            readerScheduler: PipeScheduler.Inline,
            writerScheduler: PipeScheduler.Inline));
    }

    public void SwitchEncryption(IBufferedCipher cipher)
    {
        _isEncrypted = true;
        _encryptor = cipher ?? throw new ArgumentNullException(nameof(cipher));
    }

    public override void Advance(int bytes)
    {
        if (_isEncrypted)
            _encPipe.Writer.Advance(bytes);
        else
            _pipeWriter.Advance(bytes);
    }

    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        if (_isEncrypted)
            return _encPipe.Writer.GetMemory(sizeHint);
        else
            return _pipeWriter.GetMemory(sizeHint);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        if (_isEncrypted)
            return _encPipe.Writer.GetSpan(sizeHint);
        else
            return _pipeWriter.GetSpan(sizeHint);
    }

    public override bool CanGetUnflushedBytes => _pipeWriter.CanGetUnflushedBytes;
    public override long UnflushedBytes => _pipeWriter.UnflushedBytes;

    public override async ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        if (_isEncrypted)
        {
            var flushAsync = await _encPipe.Writer.FlushAsync(cancellationToken)
                .ConfigureAwait(false);

            if (flushAsync.IsCanceled)
                return flushAsync;

            bool tryR = _encPipe.Reader.TryRead(out ReadResult result);
            Debug.Assert(tryR, $"{nameof(_encPipe.Reader.TryRead)} should return true");
            if (result.IsCanceled || result.IsCompleted)
            {
                return new FlushResult(result.IsCanceled, result.IsCompleted);
            }

            Encrypt(result.Buffer, _pipeWriter);
            _encPipe.Reader.AdvanceTo(result.Buffer.End);
        }

        return await _pipeWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private void Encrypt(in ReadOnlySequence<byte> data, PipeWriter output)
    {
        if (!_isEncrypted || _encryptor is null)
            throw new InvalidOperationException("Encryption is not initialized.");

        foreach (ReadOnlyMemory<byte> segment in data)
        {
            ReadOnlySpan<byte> src = segment.Span;
            int outSize = _encryptor.GetUpdateOutputSize(src.Length);
            Span<byte> dest = output.GetSpan(outSize);

            int written = _encryptor.ProcessBytes(src, dest);
            Debug.Assert(written <= dest.Length);
            if (written > 0)
            {
                output.Advance(written);
            }
        }
    }

    public override void CancelPendingFlush()
    {
        if (_isEncrypted)
        {
            _encPipe.Writer.CancelPendingFlush();
            _encPipe.Reader.CancelPendingRead();
        }
        else
        {
            _pipeWriter.CancelPendingFlush();
        }
    }

    public override void Complete(Exception? exception = null)
    {
        if (_isEncrypted)
        {
            _encPipe.Writer.Complete(exception);
            _encPipe.Reader.Complete(exception);
        }

        _pipeWriter.Complete(exception);
    }
}