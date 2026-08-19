using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using ArmAes = System.Runtime.Intrinsics.Arm.Aes;

namespace McProtoNet.Cryptography;

internal sealed class AesCfb8ArmCipher : PacketCipher
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

    public static bool IsSupported => ArmAes.IsSupported && AdvSimd.IsSupported;

    internal AesCfb8ArmCipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, bool encrypting)
    {
        _encrypting = encrypting;
        _register = Vector128.Create<byte>(iv[..16]);

        Span<byte> expanded = stackalloc byte[AesKeySchedule.ExpandedKey128Length];
        AesKeySchedule.ExpandKey128(key[..16], expanded);

        _rk0 = Vector128.Create<byte>(expanded[..16]);
        _rk1 = Vector128.Create<byte>(expanded[16..32]);
        _rk2 = Vector128.Create<byte>(expanded[32..48]);
        _rk3 = Vector128.Create<byte>(expanded[48..64]);
        _rk4 = Vector128.Create<byte>(expanded[64..80]);
        _rk5 = Vector128.Create<byte>(expanded[80..96]);
        _rk6 = Vector128.Create<byte>(expanded[96..112]);
        _rk7 = Vector128.Create<byte>(expanded[112..128]);
        _rk8 = Vector128.Create<byte>(expanded[128..144]);
        _rk9 = Vector128.Create<byte>(expanded[144..160]);
        _rk10 = Vector128.Create<byte>(expanded[160..176]);
        expanded.Clear();
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
                register = AdvSimd.Insert(AdvSimd.ExtractVector128(register, Vector128<byte>.Zero, 1), 15, cipherByte);
            }
        }
        else
        {
            int position = buffer.Length >= 16 ? DecryptPipelined(buffer, ref register) : 0;

            for (int i = position; i < buffer.Length; i++)
            {
                byte cipherByte = buffer[i];
                buffer[i] = (byte)(cipherByte ^ EncryptRegister(register).GetElement(0));
                register = AdvSimd.Insert(AdvSimd.ExtractVector128(register, Vector128<byte>.Zero, 1), 15, cipherByte);
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
                AdvSimd.ExtractVector128(register, cipher, 1),
                AdvSimd.ExtractVector128(register, cipher, 2),
                AdvSimd.ExtractVector128(register, cipher, 3),
                AdvSimd.ExtractVector128(register, cipher, 4),
                AdvSimd.ExtractVector128(register, cipher, 5),
                AdvSimd.ExtractVector128(register, cipher, 6),
                AdvSimd.ExtractVector128(register, cipher, 7));
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref start, position), cipher.AsUInt64().ToScalar() ^ keystream);

            keystream = KeystreamEight(
                AdvSimd.ExtractVector128(register, cipher, 8),
                AdvSimd.ExtractVector128(register, cipher, 9),
                AdvSimd.ExtractVector128(register, cipher, 10),
                AdvSimd.ExtractVector128(register, cipher, 11),
                AdvSimd.ExtractVector128(register, cipher, 12),
                AdvSimd.ExtractVector128(register, cipher, 13),
                AdvSimd.ExtractVector128(register, cipher, 14),
                AdvSimd.ExtractVector128(register, cipher, 15));
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
        RoundEight(_rk0, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk1, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk2, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk3, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk4, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk5, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk6, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk7, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);
        RoundEight(_rk8, ref b0, ref b1, ref b2, ref b3, ref b4, ref b5, ref b6, ref b7);

        Vector128<byte> rk9 = _rk9;
        Vector128<byte> rk10 = _rk10;
        b0 = ArmAes.Encrypt(b0, rk9) ^ rk10;
        b1 = ArmAes.Encrypt(b1, rk9) ^ rk10;
        b2 = ArmAes.Encrypt(b2, rk9) ^ rk10;
        b3 = ArmAes.Encrypt(b3, rk9) ^ rk10;
        b4 = ArmAes.Encrypt(b4, rk9) ^ rk10;
        b5 = ArmAes.Encrypt(b5, rk9) ^ rk10;
        b6 = ArmAes.Encrypt(b6, rk9) ^ rk10;
        b7 = ArmAes.Encrypt(b7, rk9) ^ rk10;

        Vector128<ushort> low = AdvSimd.Arm64.ZipLow(
            AdvSimd.Arm64.ZipLow(b0, b1).AsUInt16(),
            AdvSimd.Arm64.ZipLow(b2, b3).AsUInt16());
        Vector128<ushort> high = AdvSimd.Arm64.ZipLow(
            AdvSimd.Arm64.ZipLow(b4, b5).AsUInt16(),
            AdvSimd.Arm64.ZipLow(b6, b7).AsUInt16());
        return AdvSimd.Arm64.ZipLow(low.AsUInt32(), high.AsUInt32()).AsUInt64().ToScalar();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RoundEight(
        Vector128<byte> rk,
        ref Vector128<byte> b0, ref Vector128<byte> b1, ref Vector128<byte> b2, ref Vector128<byte> b3,
        ref Vector128<byte> b4, ref Vector128<byte> b5, ref Vector128<byte> b6, ref Vector128<byte> b7)
    {
        b0 = ArmAes.MixColumns(ArmAes.Encrypt(b0, rk));
        b1 = ArmAes.MixColumns(ArmAes.Encrypt(b1, rk));
        b2 = ArmAes.MixColumns(ArmAes.Encrypt(b2, rk));
        b3 = ArmAes.MixColumns(ArmAes.Encrypt(b3, rk));
        b4 = ArmAes.MixColumns(ArmAes.Encrypt(b4, rk));
        b5 = ArmAes.MixColumns(ArmAes.Encrypt(b5, rk));
        b6 = ArmAes.MixColumns(ArmAes.Encrypt(b6, rk));
        b7 = ArmAes.MixColumns(ArmAes.Encrypt(b7, rk));
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
        Vector128<byte> block = ArmAes.MixColumns(ArmAes.Encrypt(register, _rk0));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk1));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk2));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk3));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk4));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk5));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk6));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk7));
        block = ArmAes.MixColumns(ArmAes.Encrypt(block, _rk8));
        block = ArmAes.Encrypt(block, _rk9);
        return block ^ _rk10;
    }
}
