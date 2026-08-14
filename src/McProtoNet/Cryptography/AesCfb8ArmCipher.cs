using System.Runtime.CompilerServices;
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
            for (int i = 0; i < buffer.Length; i++)
            {
                byte cipherByte = buffer[i];
                buffer[i] = (byte)(cipherByte ^ EncryptRegister(register).GetElement(0));
                register = AdvSimd.Insert(AdvSimd.ExtractVector128(register, Vector128<byte>.Zero, 1), 15, cipherByte);
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
