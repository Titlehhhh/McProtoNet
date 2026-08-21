using System;
using BenchmarkDotNet.Attributes;
using McProtoNet.Transport.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Benchmark.Pipelines;

public enum CipherCore
{
    Hardware,
    OneShot,
    BouncyCastle,
    BouncyCastleX86
}

[Config(typeof(ShortRunConfig))]
[MemoryDiagnoser]
public class PacketCipherBenchmarks
{
    private static readonly byte[] Secret = "0123456789ABCDEF"u8.ToArray();

    [Params(1, 8, 16, 32, 50, 64, 128, 256, 512, 1024, 2000, 4096)] public int ChunkSize;

    [Params(CipherCore.Hardware, CipherCore.OneShot, CipherCore.BouncyCastle, CipherCore.BouncyCastleX86)]
    public CipherCore Core;

    private byte[] _buffer;
    private PacketCipher _cipher;
    private PacketCipher _decryptor;
    private IBufferedCipher _bouncyCipher;
    private IBufferedCipher _bouncyDecryptor;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _buffer = new byte[ChunkSize];
        new Random(40).NextBytes(_buffer);

        switch (Core)
        {
            case CipherCore.Hardware:
                _cipher = new AesCfb8HardwareCipher(Secret, Secret, encrypting: true);
                _decryptor = new AesCfb8HardwareCipher(Secret, Secret, encrypting: false);
                break;
            case CipherCore.OneShot:
                _cipher = new AesCfb8Cipher(Secret, Secret, encrypting: true);
                _decryptor = new AesCfb8Cipher(Secret, Secret, encrypting: false);
                break;
            case CipherCore.BouncyCastle:
                _bouncyCipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
                _bouncyCipher.Init(true, new ParametersWithIV(new KeyParameter(Secret), Secret));
                _bouncyDecryptor = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
                _bouncyDecryptor.Init(false, new ParametersWithIV(new KeyParameter(Secret), Secret));
                break;
            case CipherCore.BouncyCastleX86:
                _bouncyCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine_X86(), 8));
                _bouncyCipher.Init(true, new ParametersWithIV(new KeyParameter(Secret), Secret));
                _bouncyDecryptor = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine_X86(), 8));
                _bouncyDecryptor.Init(false, new ParametersWithIV(new KeyParameter(Secret), Secret));
                break;
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _cipher?.Dispose();
        _decryptor?.Dispose();
    }

    [Benchmark]
    public void Encrypt()
    {
        if (_bouncyCipher is not null)
        {
            _bouncyCipher.ProcessBytes(_buffer, 0, _buffer.Length, _buffer, 0);
        }
        else
        {
            _cipher.Transform(_buffer);
        }
    }

    [Benchmark]
    public void Decrypt()
    {
        if (_bouncyDecryptor is not null)
        {
            _bouncyDecryptor.ProcessBytes(_buffer, 0, _buffer.Length, _buffer, 0);
        }
        else
        {
            _decryptor.Transform(_buffer);
        }
    }
}
