namespace McProtoNet.Transport.Cryptography;

/// <summary>
/// Represents an AES/CFB8 stream cipher that transforms protocol bytes in place.
/// </summary>
/// <remarks>
/// A cipher carries the feedback register of the stream it serves, so one instance belongs to one
/// direction of one connection and must be used by one thread at a time. The concrete implementation
/// is chosen at creation time: an x86 AES-NI core, an ARM64 NEON core, or a scalar fallback.
/// </remarks>
public abstract class PacketCipher : IDisposable
{
    /// <summary>
    /// The required length of the shared secret, in bytes.
    /// </summary>
    public const int SharedSecretLength = 16;

    /// <summary>
    /// When overridden in a derived class, transforms the specified buffer in place and advances the
    /// cipher state by its length.
    /// </summary>
    /// <param name="buffer">The bytes to encrypt or decrypt in place.</param>
    /// <remarks>
    /// The bytes must be passed in wire order, without gaps, because each byte feeds the register that
    /// transforms the next one.
    /// </remarks>
    public abstract void Transform(Span<byte> buffer);

    /// <summary>
    /// Creates a cipher that encrypts outgoing bytes with the specified shared secret.
    /// </summary>
    /// <param name="sharedSecret">The shared secret, which must be exactly
    /// <see cref="SharedSecretLength"/> bytes long. It serves as both the key and the initialization
    /// vector.</param>
    /// <returns>A new <see cref="PacketCipher"/> that encrypts.</returns>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not
    /// <see cref="SharedSecretLength"/> bytes long.</exception>
    public static PacketCipher CreateEncryptor(ReadOnlySpan<byte> sharedSecret)
    {
        return Create(sharedSecret, encrypting: true);
    }

    /// <summary>
    /// Creates a cipher that decrypts incoming bytes with the specified shared secret.
    /// </summary>
    /// <param name="sharedSecret">The shared secret, which must be exactly
    /// <see cref="SharedSecretLength"/> bytes long. It serves as both the key and the initialization
    /// vector.</param>
    /// <returns>A new <see cref="PacketCipher"/> that decrypts.</returns>
    /// <exception cref="ArgumentException"><paramref name="sharedSecret"/> is not
    /// <see cref="SharedSecretLength"/> bytes long.</exception>
    public static PacketCipher CreateDecryptor(ReadOnlySpan<byte> sharedSecret)
    {
        return Create(sharedSecret, encrypting: false);
    }

    private static PacketCipher Create(ReadOnlySpan<byte> sharedSecret, bool encrypting)
    {
        ValidateSharedSecret(sharedSecret);

        if (AesCfb8HardwareCipher.IsSupported)
        {
            return new AesCfb8HardwareCipher(sharedSecret, sharedSecret, encrypting);
        }

        if (AesCfb8ArmCipher.IsSupported)
        {
            return new AesCfb8ArmCipher(sharedSecret, sharedSecret, encrypting);
        }

        return new AesCfb8Cipher(sharedSecret, sharedSecret, encrypting);
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="PacketCipher"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="PacketCipher"/> and optionally releases
    /// the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.</param>
    /// <remarks>
    /// A derived class clears its key material here.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
    }

    private static void ValidateSharedSecret(ReadOnlySpan<byte> sharedSecret)
    {
        if (sharedSecret.Length != SharedSecretLength)
        {
            throw new ArgumentException(
                $"The shared secret must be exactly {SharedSecretLength} bytes.", nameof(sharedSecret));
        }
    }
}
