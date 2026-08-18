using System.IO.Pipelines;
using McProtoNet.Cryptography;

namespace McProtoNet.Net;

public sealed class CryptoPipeWriter : PipeWriter, IDisposable
{
    private readonly PipeWriter _writer;

    private PacketCipher? _encryptor;
    private Memory<byte> _issued;
    private int _pending;
    private bool _disposed;

    public CryptoPipeWriter(PipeWriter pipeWriter)
    {
        ArgumentNullException.ThrowIfNull(pipeWriter);
        _writer = pipeWriter;
    }

    public bool EncryptionEnabled => _encryptor is not null;

    #region Encryption

    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        PacketCipher encryptor = PacketCipher.CreateEncryptor(sharedSecret);
        try
        {
            EnableEncryption(encryptor);
        }
        catch
        {
            encryptor.Dispose();
            throw;
        }
    }

    public void EnableEncryption(PacketCipher encryptor)
    {
        ArgumentNullException.ThrowIfNull(encryptor);
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_encryptor is not null)
        {
            throw new InvalidOperationException("Encryption is already enabled.");
        }

        if (_writer.CanGetUnflushedBytes && _writer.UnflushedBytes > 0)
        {
            throw new InvalidOperationException(
                "Encryption must be enabled at a packet boundary: flush the plaintext already written first.");
        }

        _encryptor = encryptor;
    }

    #endregion

    #region PipeWriter

    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _encryptor is null ? _writer.GetMemory(sizeHint) : IssueBuffer(sizeHint);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _encryptor is null ? _writer.GetSpan(sizeHint) : IssueBuffer(sizeHint).Span;
    }

    public override void Advance(int bytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_encryptor is null)
        {
            _writer.Advance(bytes);
            return;
        }

        if ((uint)bytes > (uint)(_issued.Length - _pending))
        {
            ThrowAdvanceOutOfIssuedBuffer(bytes);
        }

        _pending += bytes;
    }

    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        CommitPending();
        return _writer.FlushAsync(cancellationToken);
    }

    public override void CancelPendingFlush()
    {
        _writer.CancelPendingFlush();
    }

    public override void Complete(Exception? exception = null)
    {
        PrepareComplete(exception);
        _writer.Complete(exception);
    }

    public override ValueTask CompleteAsync(Exception? exception = null)
    {
        PrepareComplete(exception);
        return _writer.CompleteAsync(exception);
    }

    public override bool CanGetUnflushedBytes => _writer.CanGetUnflushedBytes;

    public override long UnflushedBytes => _writer.UnflushedBytes + _pending;

    #endregion

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        DiscardPending();

        PacketCipher? encryptor = _encryptor;
        _encryptor = null;
        encryptor?.Dispose();
    }

    private Memory<byte> IssueBuffer(int sizeHint)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sizeHint);

        int free = _issued.Length - _pending;
        if (free > 0 && free >= sizeHint)
        {
            return _issued[_pending..];
        }

        CommitPending();
        _issued = _writer.GetMemory(sizeHint);
        return _issued;
    }

    private void CommitPending()
    {
        int count = _pending;
        Memory<byte> issued = _issued;
        DiscardPending();

        if (count > 0)
        {
            _encryptor!.Transform(issued.Span[..count]);
            _writer.Advance(count);
        }
    }

    private void DiscardPending()
    {
        _issued = default;
        _pending = 0;
    }

    private void PrepareComplete(Exception? exception)
    {
        if (exception is null)
        {
            CommitPending();
        }
        else
        {
            DiscardPending();
        }
    }

    private void ThrowAdvanceOutOfIssuedBuffer(int bytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);

        if (_issued.IsEmpty)
        {
            throw new InvalidOperationException(
                "No writing operation is in progress: request memory with GetMemory or GetSpan first.");
        }

        throw new ArgumentOutOfRangeException(nameof(bytes), bytes,
            $"Cannot advance past the end of the issued buffer ({_issued.Length - _pending} bytes left).");
    }
}
