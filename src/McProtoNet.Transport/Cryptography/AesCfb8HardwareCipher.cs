using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using X86Aes = System.Runtime.Intrinsics.X86.Aes;


namespace McProtoNet.Transport.Cryptography;

internal sealed class AesCfb8HardwareCipher : PacketCipher
{
    private readonly bool _encrypting;
    private bool _disposed;

    private Vector128<byte> _rk0;
    private Vector128<byte> _rk1;
    private Vector128<byte> _rk2;
    private Vector128<byte> _rk3;
    private Vector128<byte> _rk4;
    private Vector128<byte> _rk5;
    private Vector128<byte> _rk6;
    private Vector128<byte> _rk7;
    private Vector128<byte> _rk8;
    private Vector128<byte> _rk9;
    private Vector128<byte> _rk10;

    private Vector128<byte> _register;

    public static bool IsSupported => X86Aes.IsSupported && Sse2.IsSupported && Ssse3.IsSupported && Sse41.IsSupported;

    internal AesCfb8HardwareCipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, bool encrypting)
    {
        _encrypting = encrypting;
        _register = Vector128.Create<byte>(iv[..16]);

        Vector128<byte> k = Vector128.Create<byte>(key[..16]);
        _rk0 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x01));
        _rk1 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x02));
        _rk2 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x04));
        _rk3 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x08));
        _rk4 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x10));
        _rk5 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x20));
        _rk6 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x40));
        _rk7 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x80));
        _rk8 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x1B));
        _rk9 = k;
        k = NextRoundKey(k, X86Aes.KeygenAssist(k, 0x36));
        _rk10 = k;
    }

    public override void Transform(Span<byte> buffer)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Vector128<byte> register = _register;

        if (_encrypting)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                byte cipherByte = (byte)(buffer[i] ^ EncryptRegister(register).GetElement(0));
                buffer[i] = cipherByte;
                register = Sse41.Insert(Sse2.ShiftRightLogical128BitLane(register, 1), cipherByte, 15);
            }
        }
        else
        {
            int position = buffer.Length >= 16 ? DecryptPipelined(buffer, ref register) : 0;

            for (int i = position; i < buffer.Length; i++)
            {
                byte cipherByte = buffer[i];
                buffer[i] = (byte)(cipherByte ^ EncryptRegister(register).GetElement(0));
                register = Sse41.Insert(Sse2.ShiftRightLogical128BitLane(register, 1), cipherByte, 15);
            }
        }

        _register = register;
    }

    private int DecryptPipelined(Span<byte> buffer, ref Vector128<byte> register)
    {
        ref byte start = ref MemoryMarshal.GetReference(buffer);
        int position = 0;

        while (buffer.Length - position >= 16)
        {
            Vector128<byte> cipher = Vector128.LoadUnsafe(ref start, (nuint)position);

            ulong keystream = KeystreamEight(
                register,
                Ssse3.AlignRight(cipher, register, 1),
                Ssse3.AlignRight(cipher, register, 2),
                Ssse3.AlignRight(cipher, register, 3),
                Ssse3.AlignRight(cipher, register, 4),
                Ssse3.AlignRight(cipher, register, 5),
                Ssse3.AlignRight(cipher, register, 6),
                Ssse3.AlignRight(cipher, register, 7));
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref start, position), cipher.AsUInt64().ToScalar() ^ keystream);

            keystream = KeystreamEight(
                Ssse3.AlignRight(cipher, register, 8),
                Ssse3.AlignRight(cipher, register, 9),
                Ssse3.AlignRight(cipher, register, 10),
                Ssse3.AlignRight(cipher, register, 11),
                Ssse3.AlignRight(cipher, register, 12),
                Ssse3.AlignRight(cipher, register, 13),
                Ssse3.AlignRight(cipher, register, 14),
                Ssse3.AlignRight(cipher, register, 15));
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref start, position + 8), cipher.AsUInt64().GetElement(1) ^ keystream);

            register = cipher;
            position += 16;
        }

        return position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong KeystreamEight(
        Vector128<byte> b0, Vector128<byte> b1, Vector128<byte> b2, Vector128<byte> b3,
        Vector128<byte> b4, Vector128<byte> b5, Vector128<byte> b6, Vector128<byte> b7)
    {
        Vector128<byte> rk = _rk0;
        b0 = Sse2.Xor(b0, rk);
        b1 = Sse2.Xor(b1, rk);
        b2 = Sse2.Xor(b2, rk);
        b3 = Sse2.Xor(b3, rk);
        b4 = Sse2.Xor(b4, rk);
        b5 = Sse2.Xor(b5, rk);
        b6 = Sse2.Xor(b6, rk);
        b7 = Sse2.Xor(b7, rk);

        RoundEight(_rk1, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk2, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk3, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk4, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk5, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk6, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk7, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk8, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk9, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);

        rk = _rk10;
        b0 = X86Aes.EncryptLast(b0, rk);
        b1 = X86Aes.EncryptLast(b1, rk);
        b2 = X86Aes.EncryptLast(b2, rk);
        b3 = X86Aes.EncryptLast(b3, rk);
        b4 = X86Aes.EncryptLast(b4, rk);
        b5 = X86Aes.EncryptLast(b5, rk);
        b6 = X86Aes.EncryptLast(b6, rk);
        b7 = X86Aes.EncryptLast(b7, rk);

        Vector128<ushort> low = Sse2.UnpackLow(
            Sse2.UnpackLow(b0, b1).AsUInt16(),
            Sse2.UnpackLow(b2, b3).AsUInt16());
        Vector128<ushort> high = Sse2.UnpackLow(
            Sse2.UnpackLow(b4, b5).AsUInt16(),
            Sse2.UnpackLow(b6, b7).AsUInt16());
        return Sse2.UnpackLow(low.AsUInt32(), high.AsUInt32()).AsUInt64().ToScalar();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RoundEight(
        Vector128<byte> rk,
        ref Vector128<byte> b0, ref Vector128<byte> b1, ref Vector128<byte> b2, ref Vector128<byte> b3,
        ref Vector128<byte> b4, ref Vector128<byte> b5, ref Vector128<byte> b6, ref Vector128<byte> b7)
    {
        b0 = X86Aes.Encrypt(b0, rk);
        b1 = X86Aes.Encrypt(b1, rk);
        b2 = X86Aes.Encrypt(b2, rk);
        b3 = X86Aes.Encrypt(b3, rk);
        b4 = X86Aes.Encrypt(b4, rk);
        b5 = X86Aes.Encrypt(b5, rk);
        b6 = X86Aes.Encrypt(b6, rk);
        b7 = X86Aes.Encrypt(b7, rk);
    }

    protected override void Dispose(bool disposing)
    {
        _disposed = true;
        _rk0 = default;
        _rk1 = default;
        _rk2 = default;
        _rk3 = default;
        _rk4 = default;
        _rk5 = default;
        _rk6 = default;
        _rk7 = default;
        _rk8 = default;
        _rk9 = default;
        _rk10 = default;
        _register = default;

        base.Dispose(disposing);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector128<byte> EncryptRegister(Vector128<byte> register)
    {
        Vector128<byte> block = Sse2.Xor(register, _rk0);
        block = X86Aes.Encrypt(block, _rk1);
        block = X86Aes.Encrypt(block, _rk2);
        block = X86Aes.Encrypt(block, _rk3);
        block = X86Aes.Encrypt(block, _rk4);
        block = X86Aes.Encrypt(block, _rk5);
        block = X86Aes.Encrypt(block, _rk6);
        block = X86Aes.Encrypt(block, _rk7);
        block = X86Aes.Encrypt(block, _rk8);
        block = X86Aes.Encrypt(block, _rk9);
        return X86Aes.EncryptLast(block, _rk10);
    }

    private static Vector128<byte> NextRoundKey(Vector128<byte> key, Vector128<byte> assist)
    {
        assist = Sse2.Shuffle(assist.AsUInt32(), 0xFF).AsByte();
        key = Sse2.Xor(key, Sse2.ShiftLeftLogical128BitLane(key, 4));
        key = Sse2.Xor(key, Sse2.ShiftLeftLogical128BitLane(key, 4));
        key = Sse2.Xor(key, Sse2.ShiftLeftLogical128BitLane(key, 4));
        return Sse2.Xor(key, assist);
    }
}
