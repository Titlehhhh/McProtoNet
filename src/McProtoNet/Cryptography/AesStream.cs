using System.Buffers;
using System.Diagnostics;

namespace McProtoNet.Cryptography;

public sealed class AesStream : Stream
{
    private const int EncryptChunkSize = 8 * 1024;

    private readonly Stream _baseStream;
    private readonly bool _leaveOpen;

    private volatile PacketCipher? _encryptor;
    private volatile PacketCipher? _decryptor;
    private volatile bool _encryptionEnabled;
    private volatile bool _disposed;

    public AesStream(Stream baseStream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(baseStream);
        _baseStream = baseStream;
        _leaveOpen = leaveOpen;
    }

    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _baseStream;
        }
    }

    public bool EncryptionEnabled => _encryptionEnabled;

    public override bool CanRead => _baseStream.CanRead;

    public override bool CanWrite => _baseStream.CanWrite;

    public override bool CanSeek => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public void SwitchEncryption(byte[] privateKey)
    {
        ArgumentNullException.ThrowIfNull(privateKey);
        EnableEncryption(privateKey);
    }

    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        ThrowIfDisposed();
        ThrowIfEncryptionEnabled();

        PacketCipher encryptor = PacketCipher.CreateEncryptor(sharedSecret);
        PacketCipher decryptor;
        try
        {
            decryptor = PacketCipher.CreateDecryptor(sharedSecret);
        }
        catch
        {
            encryptor.Dispose();
            throw;
        }

        Install(encryptor, decryptor);
    }

    public void EnableEncryption(PacketCipher encryptor, PacketCipher decryptor)
    {
        ArgumentNullException.ThrowIfNull(encryptor);
        ArgumentNullException.ThrowIfNull(decryptor);
        ThrowIfDisposed();
        ThrowIfEncryptionEnabled();

        Install(encryptor, decryptor);
    }

    public override int Read(Span<byte> buffer)
    {
        ThrowIfDisposed();

        int read = _baseStream.Read(buffer);
        PacketCipher? decryptor = _decryptor;
        if (decryptor is not null && read > 0)
        {
            decryptor.Transform(buffer[..read]);
        }

        return read;
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        PacketCipher? decryptor = _decryptor;
        if (decryptor is null)
        {
            return _baseStream.ReadAsync(buffer, cancellationToken);
        }

        return ReadAndDecryptAsync(decryptor, buffer, cancellationToken);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        ThrowIfDisposed();

        PacketCipher? encryptor = _encryptor;
        if (encryptor is null)
        {
            _baseStream.Write(buffer);
            return;
        }

        if (buffer.IsEmpty)
        {
            return;
        }

        byte[] rented = ArrayPool<byte>.Shared.Rent(Math.Min(buffer.Length, EncryptChunkSize));
        try
        {
            while (!buffer.IsEmpty)
            {
                int count = Math.Min(buffer.Length, rented.Length);
                Span<byte> chunk = rented.AsSpan(0, count);
                buffer[..count].CopyTo(chunk);
                encryptor.Transform(chunk);
                _baseStream.Write(chunk);
                buffer = buffer[count..];
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        PacketCipher? encryptor = _encryptor;
        if (encryptor is null)
        {
            return _baseStream.WriteAsync(buffer, cancellationToken);
        }

        return EncryptAndWriteAsync(encryptor, buffer, cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        return Read(buffer.AsSpan(offset, count));
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        Write(buffer.AsSpan(offset, count));
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        return WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override void Flush()
    {
        ThrowIfDisposed();
        _baseStream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _baseStream.FlushAsync(cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        DisposeCiphers();

        if (!_leaveOpen)
        {
            await _baseStream.DisposeAsync().ConfigureAwait(false);
        }

        await base.DisposeAsync().ConfigureAwait(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (disposing)
        {
            DisposeCiphers();
            if (!_leaveOpen)
            {
                _baseStream.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    private async ValueTask<int> ReadAndDecryptAsync(PacketCipher decryptor, Memory<byte> buffer,
        CancellationToken cancellationToken)
    {
        int read = await _baseStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        if (read > 0)
        {
            decryptor.Transform(buffer.Span[..read]);
        }

        return read;
    }

    private async ValueTask EncryptAndWriteAsync(PacketCipher encryptor, ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken)
    {
        if (buffer.IsEmpty)
        {
            return;
        }

        byte[] rented = ArrayPool<byte>.Shared.Rent(Math.Min(buffer.Length, EncryptChunkSize));
        try
        {
            while (!buffer.IsEmpty)
            {
                int count = Math.Min(buffer.Length, rented.Length);
                Memory<byte> chunk = rented.AsMemory(0, count);
                buffer[..count].CopyTo(chunk);
                encryptor.Transform(chunk.Span);
                await _baseStream.WriteAsync(chunk, cancellationToken).ConfigureAwait(false);
                buffer = buffer[count..];
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private void Install(PacketCipher encryptor, PacketCipher decryptor)
    {
        _decryptor = decryptor;
        _encryptor = encryptor;
        _encryptionEnabled = true;
    }

    private void DisposeCiphers()
    {
        _encryptor?.Dispose();
        _decryptor?.Dispose();
        _encryptor = null;
        _decryptor = null;
    }

    [StackTraceHidden]
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    [StackTraceHidden]
    private void ThrowIfEncryptionEnabled()
    {
        if (_encryptionEnabled)
        {
            throw new InvalidOperationException("Encryption is already enabled on this stream.");
        }
    }
}
