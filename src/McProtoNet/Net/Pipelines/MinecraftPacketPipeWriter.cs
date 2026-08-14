using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Cryptography;

namespace McProtoNet.Net;

public sealed class MinecraftPacketPipeWriter : IDisposable
{
    private const int NonWrite = 0;
    private const int Writing = 1;

    private readonly EncryptedPipeWriter _writer;

    private int _writeState = NonWrite;
    private int _compressionThreshold = -1;

    public MinecraftPacketPipeWriter(PipeWriter pipeWriter)
    {
        _writer = new EncryptedPipeWriter(pipeWriter);
    }

    public int CompressionThreshold
    {
        get => Volatile.Read(ref _compressionThreshold);
        set
        {
            ThrowIfWriting("Cannot change compression threshold while writing a packet.");
            Volatile.Write(ref _compressionThreshold, value);
        }
    }

    public bool EncryptionEnabled => _writer.IsEncrypted;

    public bool CanGetUnflushedBytes => _writer.CanGetUnflushedBytes;

    public long UnflushedBytes => _writer.UnflushedBytes;

    #region Encryption

    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        ThrowIfWriting("Cannot enable encryption while writing a packet.");
        _writer.SwitchEncryption(sharedSecret);
    }

    public void EnableEncryption(PacketCipher encryptor)
    {
        ThrowIfWriting("Cannot enable encryption while writing a packet.");
        _writer.SwitchEncryption(encryptor);
    }

    #endregion

    #region Packets

    public void WritePacket(ReadOnlySpan<byte> rawPacket)
    {
        BeginWrite();
        try
        {
            _writer.WritePacket(rawPacket, _compressionThreshold);
        }
        finally
        {
            EndWrite();
        }
    }

    public void WritePacket(int id, ReadOnlySpan<byte> body)
    {
        BeginWrite();
        try
        {
            _writer.WritePacket(id, body, _compressionThreshold);
        }
        finally
        {
            EndWrite();
        }
    }

    public void WritePacket(in ReadOnlySequence<byte> rawPacket)
    {
        BeginWrite();
        try
        {
            _writer.WritePacket(in rawPacket, _compressionThreshold);
        }
        finally
        {
            EndWrite();
        }
    }

    public void WritePacket(int id, in ReadOnlySequence<byte> body)
    {
        BeginWrite();
        try
        {
            _writer.WritePacket(id, in body, _compressionThreshold);
        }
        finally
        {
            EndWrite();
        }
    }

    public void WriteRawBytes(ReadOnlySpan<byte> bytes)
    {
        BeginWrite();
        try
        {
            _writer.Write(bytes);
        }
        finally
        {
            EndWrite();
        }
    }

    #endregion

    #region Pipe

    public ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        return _writer.FlushAsync(cancellationToken);
    }

    public void CancelPendingFlush()
    {
        _writer.CancelPendingFlush();
    }

    public void Complete(Exception? exception = null)
    {
        _writer.Complete(exception);
    }

    public ValueTask CompleteAsync(Exception? exception = null)
    {
        return _writer.CompleteAsync(exception);
    }

    #endregion

    public void Dispose()
    {
        _writer.Dispose();
    }

    private void BeginWrite()
    {
        if (Interlocked.CompareExchange(ref _writeState, Writing, NonWrite) == Writing)
        {
            throw new InvalidOperationException("Concurrent packet writing is not allowed.");
        }
    }

    private void EndWrite()
    {
        Interlocked.Exchange(ref _writeState, NonWrite);
    }

    private void ThrowIfWriting(string message)
    {
        if (Volatile.Read(ref _writeState) == Writing)
        {
            throw new InvalidOperationException(message);
        }
    }
}
