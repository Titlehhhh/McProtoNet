namespace McProtoNet.Cryptography;

public abstract class PacketCipher : IDisposable
{
    public const int SharedSecretLength = 16;

    public abstract void Transform(Span<byte> buffer);

    public static PacketCipher CreateEncryptor(ReadOnlySpan<byte> sharedSecret)
    {
        return Create(sharedSecret, encrypting: true);
    }

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
