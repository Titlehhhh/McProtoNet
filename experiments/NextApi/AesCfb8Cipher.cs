// EXPERIMENT (McProtoNet.Next) — the AES-128/CFB8 cipher the client owns.
//
// Д5 (docs/design/transport-hardening-plan-2026-08-10.md): the feedback loop runs in the
// platform's own Aes.EncryptCfb/DecryptCfb — one native call per Transform — instead of a
// hand-rolled per-byte AES-ECB loop. Measured (Measurements/CipherCost.cs): ~4x the
// throughput of the per-byte version, and faster than BouncyCastle even on its AES-NI engine.
//
// The one subtlety the critic pass caught (and rejected the naive prototype for): EncryptCfb
// is a one-shot keyed on an IV, so a streaming cipher must carry the CFB8 shift-register state
// across calls ITSELF. The register is the last 16 bytes of the CIPHERTEXT stream — the output
// when encrypting, the INPUT when decrypting — and for a call shorter than 16 bytes it is the
// old register shifted left with the new ciphertext appended. Relying on "last 16 bytes of the
// buffer" breaks on any chunk below 16 bytes, which the pumps deliver routinely. This class
// keeps the full register and advances it for any length; correctness is proven byte-for-byte
// against a one-shot whole-buffer oracle under adversarial chunking in Demo/CipherStreamingCheck.cs.

#nullable enable

using System;
using System.Security.Cryptography;

namespace McProtoNet.Next;

/// <summary>
/// AES-128/CFB8 stream cipher over <see cref="Aes.EncryptCfb(ReadOnlySpan{byte}, ReadOnlySpan{byte}, Span{byte}, PaddingMode, int)"/>
/// / <see cref="Aes.DecryptCfb(ReadOnlySpan{byte}, ReadOnlySpan{byte}, Span{byte}, PaddingMode, int)"/>,
/// carrying the CFB8 shift register across calls so any chunking of the stream yields the same
/// bytes as one continuous pass. One instance is a single direction; encryption always uses a pair.
/// </summary>
internal sealed class AesCfb8Cipher : PacketCipher
{
    private readonly Aes _aes;
    private readonly bool _encrypting;
    private readonly byte[] _register = new byte[16]; // the live CFB8 shift register (IV = shared secret)
    private byte[] _scratch = Array.Empty<byte>();     // EncryptCfb is not in-place; transform into here, copy back

    /// <summary>Creates one direction of the cipher.</summary>
    /// <param name="key">The 16-byte AES-128 key (the shared secret).</param>
    /// <param name="iv">The 16-byte initial shift register (the shared secret, per the protocol).</param>
    /// <param name="encrypting"><see langword="true"/> for the outbound (encrypting) instance, <see langword="false"/> for inbound.</param>
    public AesCfb8Cipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, bool encrypting)
    {
        _aes = Aes.Create();
        _aes.Key = key.ToArray();
        _encrypting = encrypting;
        iv[..16].CopyTo(_register);
    }

    /// <inheritdoc />
    public override void Transform(Span<byte> buffer)
    {
        int n = buffer.Length;
        if (n == 0)
            return;

        if (_scratch.Length < n)
            _scratch = new byte[n];
        Span<byte> dest = _scratch.AsSpan(0, n);

        if (_encrypting)
        {
            _aes.EncryptCfb(buffer, _register, dest, PaddingMode.None, feedbackSizeInBits: 8);
            // The register is fed the ciphertext — the OUTPUT when encrypting.
            AdvanceRegister(dest);
            dest.CopyTo(buffer);
        }
        else
        {
            _aes.DecryptCfb(buffer, _register, dest, PaddingMode.None, feedbackSizeInBits: 8);
            // The register is fed the ciphertext — the INPUT when decrypting. buffer still holds
            // it here (DecryptCfb wrote plaintext to dest, not to buffer), so read it before the
            // copy-back below overwrites it.
            AdvanceRegister(buffer);
            dest.CopyTo(buffer);
        }
    }

    // Advance the 16-byte CFB8 register by the ciphertext of this call, for any length:
    //   n >= 16 -> the last 16 ciphertext bytes are the whole new register;
    //   n <  16 -> shift the old register left by n and append the n ciphertext bytes.
    private void AdvanceRegister(ReadOnlySpan<byte> ciphertext)
    {
        int n = ciphertext.Length;
        if (n >= 16)
        {
            ciphertext[^16..].CopyTo(_register);
        }
        else
        {
            // memmove-safe: Span.CopyTo tolerates the overlap of this left shift.
            _register.AsSpan(n, 16 - n).CopyTo(_register.AsSpan(0, 16 - n));
            ciphertext.CopyTo(_register.AsSpan(16 - n));
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _aes.Dispose();
    }
}
