using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;
namespace McProtoNet.Transport.Pipelines;

public sealed class MinecraftPacketPipeWriter : IDisposable
{
    private const int Idle = 0;
    private const int Busy = 1;
    private const int Disposed = 2;

    private readonly CryptoPipeWriter _writer;

    private int _state = Idle;
    private int _compressionThreshold = -1;

    public MinecraftPacketPipeWriter(PipeWriter pipeWriter)
    {
        _writer = new CryptoPipeWriter(pipeWriter);
    }

    public int CompressionThreshold
    {
        get => Volatile.Read(ref _compressionThreshold);
        set
        {
            Enter();
            try
            {
                Volatile.Write(ref _compressionThreshold, value);
            }
            finally
            {
                Exit();
            }
        }
    }

    public bool EncryptionEnabled => _writer.EncryptionEnabled;

    public bool CanGetUnflushedBytes => _writer.CanGetUnflushedBytes;

    public long UnflushedBytes => _writer.UnflushedBytes;

    #region Encryption

    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        Enter();
        try
        {
            _writer.EnableEncryption(sharedSecret);
        }
        finally
        {
            Exit();
        }
    }

    public void EnableEncryption(PacketCipher encryptor)
    {
        Enter();
        try
        {
            _writer.EnableEncryption(encryptor);
        }
        finally
        {
            Exit();
        }
    }

    #endregion

    #region Packets

    public void WritePacket(ReadOnlySpan<byte> rawPacket)
    {
        Enter();
        try
        {
            _writer.WritePacket(rawPacket, _compressionThreshold);
        }
        finally
        {
            Exit();
        }
    }

    public void WritePacket(int id, ReadOnlySpan<byte> body)
    {
        Enter();
        try
        {
            _writer.WritePacket(id, body, _compressionThreshold);
        }
        finally
        {
            Exit();
        }
    }

    public void WritePacket(in ReadOnlySequence<byte> rawPacket)
    {
        Enter();
        try
        {
            _writer.WritePacket(in rawPacket, _compressionThreshold);
        }
        finally
        {
            Exit();
        }
    }

    public void WritePacket(int id, in ReadOnlySequence<byte> body)
    {
        Enter();
        try
        {
            _writer.WritePacket(id, in body, _compressionThreshold);
        }
        finally
        {
            Exit();
        }
    }

    public void WriteRawBytes(ReadOnlySpan<byte> bytes)
    {
        Enter();
        try
        {
            _writer.Write(bytes);
        }
        finally
        {
            Exit();
        }
    }

    #endregion

    #region Pipe

    public ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        Enter();

        ValueTask<FlushResult> flush;
        try
        {
            flush = _writer.FlushAsync(cancellationToken);
        }
        catch
        {
            Exit();
            throw;
        }

        if (flush.IsCompleted)
        {
            Exit();
            return flush;
        }

        return ExitAfter(flush);
    }

    public void CancelPendingFlush()
    {
        _writer.CancelPendingFlush();
    }

    public void Complete(Exception? exception = null)
    {
        Enter();
        try
        {
            _writer.Complete(exception);
        }
        finally
        {
            Exit();
        }
    }

    public ValueTask CompleteAsync(Exception? exception = null)
    {
        Enter();

        ValueTask complete;
        try
        {
            complete = _writer.CompleteAsync(exception);
        }
        catch
        {
            Exit();
            throw;
        }

        if (complete.IsCompleted)
        {
            Exit();
            return complete;
        }

        return ExitAfter(complete);
    }

    #endregion

    public void Dispose()
    {
        int previous = Interlocked.CompareExchange(ref _state, Disposed, Idle);
        if (previous == Busy)
        {
            throw new InvalidOperationException("Cannot dispose the writer while another operation is in progress.");
        }

        if (previous == Disposed)
        {
            return;
        }

        _writer.Dispose();
    }

    private async ValueTask<FlushResult> ExitAfter(ValueTask<FlushResult> flush)
    {
        try
        {
            return await flush.ConfigureAwait(false);
        }
        finally
        {
            Exit();
        }
    }

    private async ValueTask ExitAfter(ValueTask complete)
    {
        try
        {
            await complete.ConfigureAwait(false);
        }
        finally
        {
            Exit();
        }
    }

    private void Enter()
    {
        int previous = Interlocked.CompareExchange(ref _state, Busy, Idle);
        if (previous == Busy)
        {
            throw new InvalidOperationException("Concurrent packet writing is not allowed.");
        }

        if (previous == Disposed)
        {
            throw new ObjectDisposedException(nameof(MinecraftPacketPipeWriter));
        }
    }

    private void Exit()
    {
        Interlocked.CompareExchange(ref _state, Idle, Busy);
    }
}
