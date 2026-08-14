using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Cryptography;

namespace McProtoNet.Net;

public sealed class CryptoPipeWriter : PipeWriter, IDisposable
{
    private const int SegmentSize = 4096;

    private readonly PipeWriter _writer;
    private readonly List<Segment> _segments = [];

    private PacketCipher? _encryptor;
    private byte[]? _spare;
    private bool _borrowedFromInner = true;
    private bool _bufferIssued;
    private bool _disposed;

    private struct Segment
    {
        public byte[] Buffer;
        public int Written;
    }

    public CryptoPipeWriter(PipeWriter pipeWriter)
    {
        ArgumentNullException.ThrowIfNull(pipeWriter);
        _writer = pipeWriter;
    }

    public bool EncryptionEnabled => _encryptor is not null;

    private long PendingBytes
    {
        get
        {
            long total = 0;
            for (int i = 0; i < _segments.Count; i++)
            {
                total += _segments[i].Written;
            }

            return total;
        }
    }

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

        if (_encryptor is null)
        {
            _borrowedFromInner = true;
            return _writer.GetMemory(sizeHint);
        }

        _borrowedFromInner = false;
        _bufferIssued = true;
        Segment last = EnsureRoom(sizeHint);
        return last.Buffer.AsMemory(last.Written);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_encryptor is null)
        {
            _borrowedFromInner = true;
            return _writer.GetSpan(sizeHint);
        }

        _borrowedFromInner = false;
        _bufferIssued = true;
        Segment last = EnsureRoom(sizeHint);
        return last.Buffer.AsSpan(last.Written);
    }

    public override void Advance(int bytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);

        if (_borrowedFromInner)
        {
            _writer.Advance(bytes);
            return;
        }

        if (!_bufferIssued)
        {
            throw new InvalidOperationException(
                "No writing operation is in progress: request memory with GetMemory or GetSpan first.");
        }

        Segment last = _segments[^1];
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bytes, last.Buffer.Length - last.Written);
        last.Written += bytes;
        _segments[^1] = last;
    }

    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Drain();
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

    public override bool CanGetUnflushedBytes => _encryptor is not null || _writer.CanGetUnflushedBytes;

    public override long UnflushedBytes
    {
        get
        {
            if (_encryptor is null)
            {
                return _writer.UnflushedBytes;
            }

            long total = PendingBytes;
            if (_writer.CanGetUnflushedBytes)
            {
                total += _writer.UnflushedBytes;
            }

            return total;
        }
    }

    #endregion

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        for (int i = 0; i < _segments.Count; i++)
        {
            ArrayPool<byte>.Shared.Return(_segments[i].Buffer);
        }

        _segments.Clear();

        if (_spare is not null)
        {
            ArrayPool<byte>.Shared.Return(_spare);
            _spare = null;
        }

        PacketCipher? encryptor = _encryptor;
        _encryptor = null;
        encryptor?.Dispose();
    }

    private void PrepareComplete(Exception? exception)
    {
        if (_disposed)
        {
            return;
        }

        if (exception is null)
        {
            Drain();
        }
        else
        {
            DiscardPending();
        }
    }

    private void Drain()
    {
        PacketCipher? encryptor = _encryptor;
        _bufferIssued = false;

        if (encryptor is null || _segments.Count == 0)
        {
            return;
        }

        while (_segments.Count > 0)
        {
            Segment segment = _segments[0];
            _segments.RemoveAt(0);

            if (segment.Written == 0)
            {
                Recycle(segment.Buffer);
                continue;
            }

            Span<byte> data = segment.Buffer.AsSpan(0, segment.Written);
            try
            {
                encryptor.Transform(data);
                _writer.Write(data);
            }
            finally
            {
                Recycle(segment.Buffer);
            }
        }
    }

    private void DiscardPending()
    {
        _bufferIssued = false;

        for (int i = 0; i < _segments.Count; i++)
        {
            Recycle(_segments[i].Buffer);
        }

        _segments.Clear();
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

    private Segment EnsureRoom(int sizeHint)
    {
        if (sizeHint <= 0)
        {
            sizeHint = 1;
        }

        if (_segments.Count > 0)
        {
            Segment last = _segments[^1];
            if (last.Buffer.Length - last.Written >= sizeHint)
            {
                return last;
            }
        }

        byte[] buffer;
        if (_spare is not null && _spare.Length >= sizeHint)
        {
            buffer = _spare;
            _spare = null;
        }
        else
        {
            buffer = ArrayPool<byte>.Shared.Rent(Math.Max(SegmentSize, sizeHint));
        }

        Segment next = new() { Buffer = buffer, Written = 0 };
        _segments.Add(next);
        return next;
    }
}
