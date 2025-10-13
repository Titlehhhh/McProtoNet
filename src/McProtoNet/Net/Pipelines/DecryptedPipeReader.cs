using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using Org.BouncyCastle.Crypto;

namespace McProtoNet.Net;

public sealed class DecryptedPipeReader : PipeReader
{
    private readonly PipeReader _pipeReader;

    private readonly Pipe _encPipe;
    
    
    private IBufferedCipher? _decryptor;
    private bool _isEncrypted;

    public DecryptedPipeReader(PipeReader baseReader, Pipe? pipe = null)
    {
        _pipeReader = baseReader;
        _encPipe = pipe ?? new Pipe(
            new PipeOptions(
                readerScheduler: PipeScheduler.Inline,
                writerScheduler: PipeScheduler.Inline));
    }

    public void SwitchEncryption(IBufferedCipher cipher)
    {
        _isEncrypted = true;
        _decryptor = cipher;
    }

    public override bool TryRead(out ReadResult result)
    {
        if (_isEncrypted)
        {
            if (_pipeReader.TryRead(out var r1))
            {
                if (r1.IsCompleted)
                {
                    _encPipe.Writer.Complete();
                }

                Decrypt(r1.Buffer, _encPipe.Writer);
                _pipeReader.AdvanceTo(r1.Buffer.End);
                ValueTask<FlushResult> vTask = _encPipe.Writer.FlushAsync();
                if (!vTask.IsCompleted)
                {
                    throw new InvalidOperationException();
                }
            }

            result = default;
            return false;
        }

        return _pipeReader.TryRead(out result);
    }

    private void Decrypt(in ReadOnlySequence<byte> data, PipeWriter outBuff)
    {
        if (!_isEncrypted || _decryptor is null)
            throw new InvalidOperationException();

        foreach (ReadOnlyMemory<byte> segment in data)
        {
            ReadOnlySpan<byte> segSpan = segment.Span;
            int outSize = _decryptor.GetUpdateOutputSize(segSpan.Length);
            Span<byte> outSpan = outBuff.GetSpan(outSize);
            int written = _decryptor.ProcessBytes(segSpan, outSpan);

            if (written > 0)
            {
                Debug.Assert(written < outSpan.Length,
                    $"The decrypted number of bytes is less than the input buffer." +
                    $" Expected: {segSpan.Length} Actual: {written}");

                outBuff.Advance(written);
            }
        }
    }

    private void ProcessSegment(in ReadOnlySpan<byte> input, PipeWriter writer)
    {
        Span<byte> destination = writer.GetSpan();

        int len = _decryptor.ProcessBytes(input, destination);

        // Fast path
        if (input.Length <= destination.Length)
        {
            input.CopyTo(destination);
            writer.Advance(input.Length);
        }
        else
        {
        }
    }

    public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if (!_isEncrypted)
            return await
                _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);


        ReadResult result = await _pipeReader.ReadAsync(cancellationToken)
            .ConfigureAwait(false);

        if (result.IsCompleted)
        {
            await _encPipe.Writer.CompleteAsync().ConfigureAwait(false);
            return result;
        }

        Decrypt(result.Buffer, _encPipe.Writer);
        _pipeReader.AdvanceTo(result.Buffer.End);
        await _encPipe.Writer.FlushAsync(cancellationToken)
            .ConfigureAwait(false);

        return await _encPipe.Reader.ReadAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public override void AdvanceTo(SequencePosition consumed)
    {
        if (_isEncrypted)
        {
            _encPipe.Reader.AdvanceTo(consumed);
        }
        else
        {
            _pipeReader.AdvanceTo(consumed);
        }
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        if (_isEncrypted)
        {
            _encPipe.Reader.AdvanceTo(consumed, examined);
        }
        else
        {
            _pipeReader.AdvanceTo(consumed, examined);
        }
    }

    public override void CancelPendingRead()
    {
        if (_isEncrypted)
        {
            _encPipe.Reader.CancelPendingRead();
        }
        else
        {
            _pipeReader.CancelPendingRead();
        }
    }

    public override void Complete(Exception? exception = null)
    {
        if (_isEncrypted)
        {
            _encPipe.Reader.Complete(exception);
        }
        else
        {
            _pipeReader.Complete(exception);
        }
    }
}