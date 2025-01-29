using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace McProtoNet.Cryptography;

/// <summary>
/// A Stream implementation that provides AES encryption/decryption capabilities
/// </summary>
public sealed class AesStream : Stream
{
    private bool _disposed;

    /// <summary>
    /// The underlying stream being wrapped
    /// </summary>
    internal Stream BaseStream;

    /// <summary>
    /// Creates a new AesStream wrapping the provided stream
    /// </summary>
    /// <param name="stream">The stream to wrap</param>
    /// <exception cref="ArgumentNullException">Thrown when stream is null</exception>
    public AesStream(Stream stream)
    {
#if NETSTANDARD2_0
            if(stream is null)
                throw new ArgumentNullException(nameof(stream));
#else
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));
#endif
        BaseStream = stream;
    }

    /// <summary>
    /// Gets whether encryption is currently enabled
    /// </summary>
    public bool EncryptionEnabled { get; } = false;

    /// <inheritdoc/>
    public override bool CanRead => BaseStream.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => BaseStream.CanSeek;

    /// <inheritdoc/>
    public override bool CanWrite => BaseStream.CanWrite;

    /// <inheritdoc/>
    public override long Length => BaseStream.Length;

    /// <inheritdoc/>
    public override long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    /// <summary>
    /// The cipher used for encryption
    /// </summary>
    private IBufferedCipher EncryptCipher { get; set; }

    /// <summary>
    /// The cipher used for decryption
    /// </summary>
    private IBufferedCipher DecryptCipher { get; set; }

    
    /// <summary>
    /// Enables AES encryption on the stream using the provided key
    /// </summary>
    /// <param name="privatekey">The private key to use for encryption</param>
    /// <exception cref="InvalidOperationException">Thrown if encryption is already enabled</exception>
    public void SwitchEncryption(byte[] privatekey)
    {
        if (EncryptionEnabled) throw new InvalidOperationException("Шифрование уже включено");

        EncryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        EncryptCipher.Init(true, new ParametersWithIV(new KeyParameter(privatekey), privatekey, 0, 16));

        DecryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
        DecryptCipher.Init(false, new ParametersWithIV(new KeyParameter(privatekey), privatekey, 0, 16));

        BaseStream = new AsyncCipherStream(BaseStream, DecryptCipher, EncryptCipher);
    }

    /// <inheritdoc/>
    public override int Read(Span<byte> buffer)
    {
        return BaseStream.Read(buffer);
    }

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        BaseStream.Write(buffer);
    }

    /// <inheritdoc/>
    public override void CopyTo(Stream destination, int bufferSize)
    {
        BaseStream.CopyTo(destination, bufferSize);
    }

    /// <inheritdoc/>
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    /// <inheritdoc/>
    public override int ReadByte()
    {
        return BaseStream.ReadByte();
    }

    /// <inheritdoc/>
    public override int EndRead(IAsyncResult asyncResult)
    {
        return BaseStream.EndRead(asyncResult);
    }

    /// <inheritdoc/>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback,
        object? state)
    {
        return BaseStream.BeginRead(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback,
        object? state)
    {
        return BaseStream.BeginWrite(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    public override void EndWrite(IAsyncResult asyncResult)
    {
        BaseStream.EndWrite(asyncResult);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return BaseStream.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return BaseStream.GetHashCode();
    }

    /// <inheritdoc/>
    public override void Flush()
    {
        BaseStream.Flush();
    }

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return BaseStream.FlushAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        return BaseStream.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return BaseStream.ReadAsync(buffer, cancellationToken);
    }

    /// <inheritdoc/>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return BaseStream.Seek(offset, origin);
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        BaseStream.SetLength(value);
    }

    /// <inheritdoc/>
    public override void WriteByte(byte value)
    {
        BaseStream.WriteByte(value);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        BaseStream.Write(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return BaseStream.WriteAsync(buffer, cancellationToken);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        BaseStream.Dispose();

        _disposed = true;
    }
}