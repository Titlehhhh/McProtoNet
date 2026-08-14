using System.Security.Cryptography;

namespace McProtoNet.Cryptography;

internal sealed class AesCfb8Cipher : PacketCipher
{
    private const int RegisterLength = 16;
    private const int ChunkSize = 4096;

    private readonly Aes _aes;
    private readonly bool _encrypting;
    private readonly byte[] _register = new byte[RegisterLength];
    private readonly byte[] _scratch = new byte[ChunkSize];

    internal AesCfb8Cipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, bool encrypting)
    {
        _aes = Aes.Create();
        _aes.Key = key.ToArray();
        _encrypting = encrypting;
        iv[..RegisterLength].CopyTo(_register);
    }

    public override void Transform(Span<byte> buffer)
    {
        while (buffer.Length > 0)
        {
            Span<byte> chunk = buffer[..Math.Min(ChunkSize, buffer.Length)];
            Span<byte> destination = _scratch.AsSpan(0, chunk.Length);

            if (_encrypting)
            {
                _aes.EncryptCfb(chunk, _register, destination, PaddingMode.None, feedbackSizeInBits: 8);
                AdvanceRegister(destination);
            }
            else
            {
                _aes.DecryptCfb(chunk, _register, destination, PaddingMode.None, feedbackSizeInBits: 8);
                AdvanceRegister(chunk);
            }

            destination.CopyTo(chunk);
            buffer = buffer[chunk.Length..];
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _aes.Dispose();
            Array.Clear(_register);
            Array.Clear(_scratch);
        }

        base.Dispose(disposing);
    }

    private void AdvanceRegister(ReadOnlySpan<byte> ciphertext)
    {
        int length = ciphertext.Length;

        if (length >= RegisterLength)
        {
            ciphertext[^RegisterLength..].CopyTo(_register);
            return;
        }

        int kept = RegisterLength - length;
        _register.AsSpan(length, kept).CopyTo(_register.AsSpan(0, kept));
        ciphertext.CopyTo(_register.AsSpan(kept));
    }
}
