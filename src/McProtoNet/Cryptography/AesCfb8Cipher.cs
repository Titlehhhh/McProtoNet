using System.Security.Cryptography;

namespace McProtoNet.Cryptography;

internal sealed class AesCfb8Cipher : PacketCipher
{
    private const int RegisterLength = 16;

    private readonly Aes _aes;
    private readonly bool _encrypting;
    private readonly byte[] _register = new byte[RegisterLength];

    private byte[] _scratch = [];

    internal AesCfb8Cipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, bool encrypting)
    {
        _aes = Aes.Create();
        _aes.Key = key.ToArray();
        _encrypting = encrypting;
        iv[..RegisterLength].CopyTo(_register);
    }

    public override void Transform(Span<byte> buffer)
    {
        int length = buffer.Length;
        if (length == 0)
        {
            return;
        }

        if (_scratch.Length < length)
        {
            _scratch = new byte[length];
        }

        Span<byte> destination = _scratch.AsSpan(0, length);

        if (_encrypting)
        {
            _aes.EncryptCfb(buffer, _register, destination, PaddingMode.None, feedbackSizeInBits: 8);
            AdvanceRegister(destination);
        }
        else
        {
            _aes.DecryptCfb(buffer, _register, destination, PaddingMode.None, feedbackSizeInBits: 8);
            AdvanceRegister(buffer);
        }

        destination.CopyTo(buffer);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _aes.Dispose();
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
