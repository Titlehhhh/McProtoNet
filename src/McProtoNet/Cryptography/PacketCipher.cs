namespace McProtoNet.Cryptography;

public abstract class PacketCipher : IDisposable
{
    public const int SharedSecretLength = 16;

    public abstract void Transform(Span<byte> buffer);

    public static PacketCipher CreateEncryptor(ReadOnlySpan<byte> sharedSecret)
    {
        ValidateSharedSecret(sharedSecret);
        return new AesCfb8Cipher(sharedSecret, sharedSecret, encrypting: true);
    }

    public static PacketCipher CreateDecryptor(ReadOnlySpan<byte> sharedSecret)
    {
        ValidateSharedSecret(sharedSecret);
        return new AesCfb8Cipher(sharedSecret, sharedSecret, encrypting: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

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
