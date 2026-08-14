using System;
using BenchmarkDotNet.Attributes;
using McProtoNet.Cryptography;
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

[Config(typeof(CryptoPipeWriterBenchConfig))]
[MemoryDiagnoser]
public class PacketCipherBenchmarks
{
    private static readonly byte[] Secret = "0123456789ABCDEF"u8.ToArray();

    [Params(16, 128, 512, 4096)] public int ChunkSize;

    [Params(CipherCore.Hardware, CipherCore.OneShot, CipherCore.BouncyCastle, CipherCore.BouncyCastleX86)]
    public CipherCore Core;

    private byte[] _buffer;
    private PacketCipher _cipher;
    private IBufferedCipher _bouncyCipher;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _buffer = new byte[ChunkSize];
        new Random(40).NextBytes(_buffer);

        switch (Core)
        {
            case CipherCore.Hardware:
                _cipher = new AesCfb8HardwareCipher(Secret, Secret, encrypting: true);
                break;
            case CipherCore.OneShot:
                _cipher = new AesCfb8Cipher(Secret, Secret, encrypting: true);
                break;
            case CipherCore.BouncyCastle:
                _bouncyCipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
                _bouncyCipher.Init(true, new ParametersWithIV(new KeyParameter(Secret), Secret));
                break;
            case CipherCore.BouncyCastleX86:
                _bouncyCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine_X86(), 8));
                _bouncyCipher.Init(true, new ParametersWithIV(new KeyParameter(Secret), Secret));
                break;
        }
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _cipher?.Dispose();
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
}
