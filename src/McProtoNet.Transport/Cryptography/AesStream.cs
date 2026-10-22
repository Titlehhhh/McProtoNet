using System.Buffers;
using System.Diagnostics;

namespace McProtoNet.Transport.Cryptography;

/// <summary>
/// Provides a stream that encrypts what is written to an underlying stream and decrypts what is read
/// from it, once encryption is enabled.
/// </summary>
/// <remarks>
/// Until <see cref="EnableEncryption(ReadOnlySpan{byte})"/> or
/// <see cref="EnableEncryption(PacketCipher, PacketCipher)"/> is called, every read and write passes
/// through unchanged. Encryption can be enabled only once. The stream does not support seeking. Reads
/// and writes each advance their own cipher, so one thread at a time may read and one thread at a time
/// may write.
/// </remarks>
public sealed class AesStream : Stream
{
    private const int EncryptChunkSize = 8 * 1024;

    private readonly Stream _baseStream;
    private readonly bool _leaveOpen;

    private volatile PacketCipher? _encryptor;
    private volatile PacketCipher? _decryptor;
    private volatile bool _encryptionEnabled;
    private volatile bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AesStream"/> class over the specified stream.
    /// </summary>
    /// <param name="baseStream">The stream to read from and write to.</param>
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open when this instance is
    /// disposed; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseStream"/> is
    /// <see langword="null"/>.</exception>
    public AesStream(Stream baseStream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(baseStream);
        _baseStream = baseStream;
        _leaveOpen = leaveOpen;
    }

    /// <summary>
    /// Gets the stream that this instance reads from and writes to.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    public Stream BaseStream
    {
        get
        {
            ThrowIfDisposed();
            return _baseStream;
        }
    }

    /// <summary>
    /// Gets a value indicating whether encryption is enabled on the stream.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if encryption has been enabled; otherwise, <see langword="false"/>.
    /// </value>
    public bool EncryptionEnabled => _encryptionEnabled;

    /// <inheritdoc />
    public override bool CanRead => _baseStream.CanRead;

    /// <inheritdoc />
    public override bool CanWrite => _baseStream.CanWrite;

    /// <summary>
    /// Gets a value indicating whether the stream supports seeking.
    /// </summary>
    /// <value>
    /// Always <see langword="false"/>.
    /// </value>
    public override bool CanSeek => false;

    /// <summary>
    /// Gets the length of the stream. This property is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">In all cases.</exception>
    public override long Length => throw new NotSupportedException();

    /// <summary>
    /// Gets or sets the position within the stream. This property is not supported.
    /// </summary>
    /// <exception cref="NotSupportedException">In all cases.</exception>
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Enables encryption with a pair of AES/CFB8 ciphers created from the specified shared secret.
    /// </summary>
    /// <param name="sharedSecret">The shared secret, which must be exactly
    /// <see cref="PacketCipher.SharedSecretLength"/> bytes long. It serves as both the key and the
    /// initialization vector.</param>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not
    /// <see cref="PacketCipher.SharedSecretLength"/> bytes long.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled on this
    /// stream.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <remarks>
    /// Encryption applies to every read and write that starts after this call. The stream owns the
    /// ciphers and disposes them with itself.
    /// </remarks>
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

    /// <summary>
    /// Enables encryption with the specified ciphers.
    /// </summary>
    /// <param name="encryptor">The cipher applied to everything written to the stream.</param>
    /// <param name="decryptor">The cipher applied to everything read from the stream.</param>
    /// <exception cref="ArgumentNullException"><paramref name="encryptor"/> is <see langword="null"/>.
    /// -or-
    /// <paramref name="decryptor"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Encryption is already enabled on this
    /// stream.</exception>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <remarks>
    /// Encryption applies to every read and write that starts after this call. The stream takes
    /// ownership of both ciphers and disposes them with itself.
    /// </remarks>
    public void EnableEncryption(PacketCipher encryptor, PacketCipher decryptor)
    {
        ArgumentNullException.ThrowIfNull(encryptor);
        ArgumentNullException.ThrowIfNull(decryptor);
        ThrowIfDisposed();
        ThrowIfEncryptionEnabled();

        Install(encryptor, decryptor);
    }

    /// <summary>
    /// Reads a sequence of bytes from the underlying stream and decrypts them in place.
    /// </summary>
    /// <param name="buffer">The region of memory to write the data into.</param>
    /// <returns>The number of bytes read into <paramref name="buffer"/>, or 0 when the end of the
    /// stream is reached.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
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

    /// <summary>
    /// Asynchronously reads a sequence of bytes from the underlying stream and decrypts them in place.
    /// </summary>
    /// <param name="buffer">The region of memory to write the data into.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default
    /// value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous read operation. The result contains the number
    /// of bytes read, or 0 when the end of the stream is reached.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
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

    /// <summary>
    /// Encrypts a sequence of bytes and writes them to the underlying stream.
    /// </summary>
    /// <param name="buffer">The region of memory to write to the stream.</param>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <remarks>
    /// <paramref name="buffer"/> is not modified; the bytes are encrypted in a pooled buffer of at
    /// most 8192 bytes and written in as many chunks as that takes.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously encrypts a sequence of bytes and writes them to the underlying stream.
    /// </summary>
    /// <param name="buffer">The region of memory to write to the stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default
    /// value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
    /// <remarks>
    /// <paramref name="buffer"/> is not modified; the bytes are encrypted in a pooled buffer of at
    /// most 8192 bytes and written in as many chunks as that takes. A cancellation between chunks
    /// leaves the cipher advanced past the bytes that were already sent.
    /// </remarks>
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

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        return Read(buffer.AsSpan(offset, count));
    }

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        Write(buffer.AsSpan(offset, count));
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        return WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    /// <summary>
    /// Flushes the underlying stream.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    public override void Flush()
    {
        ThrowIfDisposed();
        _baseStream.Flush();
    }

    /// <summary>
    /// Asynchronously flushes the underlying stream.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    /// <exception cref="ObjectDisposedException">The current instance has already been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return _baseStream.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Sets the position within the stream. This method is not supported.
    /// </summary>
    /// <param name="offset">This parameter is not used.</param>
    /// <param name="origin">This parameter is not used.</param>
    /// <returns>This method does not return a value.</returns>
    /// <exception cref="NotSupportedException">In all cases.</exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Sets the length of the stream. This method is not supported.
    /// </summary>
    /// <param name="value">This parameter is not used.</param>
    /// <exception cref="NotSupportedException">In all cases.</exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Asynchronously releases all resources used by the current instance of the
    /// <see cref="AesStream"/> class.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>
    /// The ciphers are disposed. The underlying stream is disposed unless this instance was created
    /// with <c>leaveOpen</c> set to <see langword="true"/>.
    /// </remarks>
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

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="AesStream"/> and optionally releases the
    /// managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.</param>
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
