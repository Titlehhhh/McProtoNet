using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using X86Aes = System.Runtime.Intrinsics.X86.Aes;


namespace McProtoNet.Cryptography;

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

    public static bool IsSupported => X86Aes.IsSupported && Sse2.IsSupported && Sse41.IsSupported;

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
            for (int i = 0; i < buffer.Length; i++)
            {
                byte cipherByte = buffer[i];
                buffer[i] = (byte)(cipherByte ^ EncryptRegister(register).GetElement(0));
                register = Sse41.Insert(Sse2.ShiftRightLogical128BitLane(register, 1), cipherByte, 15);
            }
        }

        _register = register;
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
