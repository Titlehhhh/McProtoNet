using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Cryptography;

namespace McProtoNet.Net;

public sealed class EncryptedPipeWriter : PipeWriter, IDisposable
{
    private const int SegmentSize = 4096;

    private readonly PipeWriter _writer;
    private readonly List<byte[]> _segments = [];
    private readonly List<int> _sealedLengths = [];

    private PacketCipher? _encryptor;
    private int _position;
    private bool _borrowedFromInner = true;

    public EncryptedPipeWriter(PipeWriter pipeWriter)
    {
        ArgumentNullException.ThrowIfNull(pipeWriter);
        _writer = pipeWriter;
    }

    public bool IsEncrypted => _encryptor is not null;

    public long PendingBytes
    {
        get
        {
            long total = _position;
            for (int i = 0; i < _sealedLengths.Count; i++)
            {
                total += _sealedLengths[i];
            }

            return total;
        }
    }

    #region Encryption

    public void SwitchEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        SwitchEncryption(PacketCipher.CreateEncryptor(sharedSecret));
    }

    public void SwitchEncryption(PacketCipher encryptor)
    {
        ArgumentNullException.ThrowIfNull(encryptor);

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
        if (_encryptor is null)
        {
            _borrowedFromInner = true;
            return _writer.GetMemory(sizeHint);
        }

        _borrowedFromInner = false;
        EnsureRoom(sizeHint);
        return _segments[^1].AsMemory(_position);
    }

    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        if (_encryptor is null)
        {
            _borrowedFromInner = true;
            return _writer.GetSpan(sizeHint);
        }

        _borrowedFromInner = false;
        EnsureRoom(sizeHint);
        return _segments[^1].AsSpan(_position);
    }

    public override void Advance(int bytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(bytes);

        if (_borrowedFromInner)
        {
            _writer.Advance(bytes);
            return;
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(bytes, _segments[^1].Length - _position);
        _position += bytes;
    }

    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        Drain();
        return _writer.FlushAsync(cancellationToken);
    }

    public override void CancelPendingFlush()
    {
        _writer.CancelPendingFlush();
    }

    public override void Complete(Exception? exception = null)
    {
        if (exception is null)
        {
            Drain();
        }
        else
        {
            Reset();
        }

        _writer.Complete(exception);
    }

    public override ValueTask CompleteAsync(Exception? exception = null)
    {
        if (exception is null)
        {
            Drain();
        }
        else
        {
            Reset();
        }

        return _writer.CompleteAsync(exception);
    }

    public override bool CanGetUnflushedBytes => _writer.CanGetUnflushedBytes;

    public override long UnflushedBytes => _writer.UnflushedBytes + PendingBytes;

    #endregion

    public void Dispose()
    {
        for (int i = 0; i < _segments.Count; i++)
        {
            ArrayPool<byte>.Shared.Return(_segments[i]);
        }

        _segments.Clear();
        _sealedLengths.Clear();
        _position = 0;

        PacketCipher? encryptor = _encryptor;
        _encryptor = null;
        encryptor?.Dispose();
    }

    private void Drain()
    {
        PacketCipher? encryptor = _encryptor;
        if (encryptor is null || _segments.Count == 0)
        {
            return;
        }

        for (int i = 0; i < _segments.Count; i++)
        {
            int length = i < _sealedLengths.Count ? _sealedLengths[i] : _position;
            if (length == 0)
            {
                continue;
            }

            Span<byte> segment = _segments[i].AsSpan(0, length);
            encryptor.Transform(segment);
            _writer.Write(segment);
        }

        Reset();
    }

    private void Reset()
    {
        for (int i = _segments.Count - 1; i >= 1; i--)
        {
            ArrayPool<byte>.Shared.Return(_segments[i]);
            _segments.RemoveAt(i);
        }

        if (_segments.Count == 1 && _segments[0].Length > SegmentSize)
        {
            ArrayPool<byte>.Shared.Return(_segments[0]);
            _segments.Clear();
        }

        _sealedLengths.Clear();
        _position = 0;
    }

    private void EnsureRoom(int sizeHint)
    {
        if (sizeHint <= 0)
        {
            sizeHint = 1;
        }

        if (_segments.Count > 0 && _segments[^1].Length - _position >= sizeHint)
        {
            return;
        }

        if (_segments.Count > 0)
        {
            _sealedLengths.Add(_position);
        }

        _segments.Add(ArrayPool<byte>.Shared.Rent(Math.Max(SegmentSize, sizeHint)));
        _position = 0;
    }
}
