using McProtoNet.Transport.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Cryptography;

public class PacketCipherTests(ITestOutputHelper output)
{
    private static readonly byte[] TestKey = "0123456789ABCDEF"u8.ToArray();

    private static readonly int[] ChunkPattern = [1, 7, 15, 16, 17, 50, 512, 4095, 4096, 4097];

    public static TheoryData<string> Cores => new() { "hardware", "arm", "fallback" };

    private static PacketCipher CreateCore(string core, bool encrypting)
    {
        return CreateCore(core, TestKey, TestKey, encrypting);
    }

    private static PacketCipher CreateCore(string core, byte[] key, byte[] iv, bool encrypting)
    {
        if (core == "hardware")
        {
            Assert.SkipUnless(AesCfb8HardwareCipher.IsSupported, "x86 AES instructions are not available");
            return new AesCfb8HardwareCipher(key, iv, encrypting);
        }

        if (core == "arm")
        {
            Assert.SkipUnless(AesCfb8ArmCipher.IsSupported, "ARM AES instructions are not available");
            return new AesCfb8ArmCipher(key, iv, encrypting);
        }

        return new AesCfb8Cipher(key, iv, encrypting);
    }

    private static byte[] ReferenceTransform(bool forEncryption, byte[] data)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(TestKey), TestKey));
        byte[] output = new byte[cipher.GetOutputSize(data.Length)];
        int written = cipher.ProcessBytes(data, 0, data.Length, output, 0);
        return output.AsSpan(0, written).ToArray();
    }

    private static byte[] RandomBytes(int count)
    {
        byte[] data = new byte[count];
        new Random(40).NextBytes(data);
        return data;
    }

    private static void TransformInChunks(PacketCipher cipher, byte[] data)
    {
        int position = 0;
        int chunkIndex = 0;
        while (position < data.Length)
        {
            int length = Math.Min(ChunkPattern[chunkIndex++ % ChunkPattern.Length], data.Length - position);
            cipher.Transform(data.AsSpan(position, length));
            position += length;
        }
    }

    [Fact]
    public void HardwareCore_ShouldBeUsed_WhenSupported()
    {
        Assert.SkipUnless(AesCfb8HardwareCipher.IsSupported, "x86 AES instructions are not available");
        using var cipher = PacketCipher.CreateEncryptor(TestKey);
        Assert.IsType<AesCfb8HardwareCipher>(cipher);
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Encrypt_ShouldMatchNistVectors(string core)
    {
        byte[] key = Convert.FromHexString("2B7E151628AED2A6ABF7158809CF4F3C");
        byte[] iv = Convert.FromHexString("000102030405060708090A0B0C0D0E0F");
        byte[] plain = Convert.FromHexString("6BC1BEE22E409F96E93D7E117393172AAE2D");
        byte[] expected = Convert.FromHexString("3B79424C9C0DD436BACE9E0ED4586A4F32B9");

        byte[] data = plain.ToArray();
        using (var encryptor = CreateCore(core, key, iv, encrypting: true))
        {
            encryptor.Transform(data);
        }

        Assert.Equal(expected, data);

        using (var decryptor = CreateCore(core, key, iv, encrypting: false))
        {
            decryptor.Transform(data);
        }

        Assert.Equal(plain, data);

        byte[] single = [0x61];
        using (var encryptor = CreateCore(core,
                   Convert.FromHexString("C57D699D89DF7CFBEF71C080A6B10AC3"),
                   Convert.FromHexString("FCB2BC4C006B87483978796A2AE2C42E"),
                   encrypting: true))
        {
            encryptor.Transform(single);
        }

        Assert.Equal(new byte[] { 0x24 }, single);
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Encrypt_ShouldMatchReference_OnLongRandomChunkStream(string core)
    {
        byte[] plain = RandomBytes(1_000_000);
        byte[] actual = plain.ToArray();

        var lengths = new Random(41);
        using var cipher = CreateCore(core, encrypting: true);
        int position = 0;
        while (position < actual.Length)
        {
            int length = Math.Min(1 + lengths.Next(5000), actual.Length - position);
            cipher.Transform(actual.AsSpan(position, length));
            position += length;
        }

        Assert.Equal(ReferenceTransform(true, plain), actual);
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Decrypt_ShouldMatchReference_OnLongRandomChunkStream(string core)
    {
        byte[] encrypted = ReferenceTransform(true, RandomBytes(1_000_000));
        byte[] actual = encrypted.ToArray();

        var lengths = new Random(42);
        using var cipher = CreateCore(core, encrypting: false);
        int position = 0;
        while (position < actual.Length)
        {
            int length = Math.Min(1 + lengths.Next(5000), actual.Length - position);
            cipher.Transform(actual.AsSpan(position, length));
            position += length;
        }

        Assert.Equal(ReferenceTransform(false, encrypted), actual);
    }

    [Fact]
    public void ExpandKey128_ShouldMatchFips197Vector()
    {
        byte[] key = Convert.FromHexString("2B7E151628AED2A6ABF7158809CF4F3C");
        byte[] expanded = new byte[AesKeySchedule.ExpandedKey128Length];

        AesKeySchedule.ExpandKey128(key, expanded);

        Assert.Equal(key, expanded[..16]);
        Assert.Equal(
            Convert.FromHexString("A0FAFE1788542CB123A339392A6C7605"),
            expanded[16..32]);
        Assert.Equal(
            Convert.FromHexString("D014F9A8C9EE2589E13F0CC8B6630CA6"),
            expanded[160..176]);
    }

    [Fact]
    public void Transform_ShouldThrow_AfterDispose()
    {
        var cipher = PacketCipher.CreateEncryptor(TestKey);
        cipher.Dispose();

        byte[] data = new byte[16];
        Assert.ThrowsAny<ObjectDisposedException>(() => cipher.Transform(data));
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Encrypt_ShouldMatchReference_AcrossChunkBoundaries(string core)
    {
        byte[] plain = RandomBytes(20_000);
        byte[] actual = plain.ToArray();

        using var cipher = CreateCore(core, encrypting: true);
        TransformInChunks(cipher, actual);

        Assert.Equal(ReferenceTransform(true, plain), actual);
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Decrypt_ShouldMatchReference_AcrossChunkBoundaries(string core)
    {
        byte[] encrypted = ReferenceTransform(true, RandomBytes(20_000));
        byte[] actual = encrypted.ToArray();

        using var cipher = CreateCore(core, encrypting: false);
        TransformInChunks(cipher, actual);

        Assert.Equal(ReferenceTransform(false, encrypted), actual);
    }

    [Fact]
    public void Cores_ShouldRoundTrip_EachOther()
    {
        byte[] plain = RandomBytes(10_000);
        byte[] data = plain.ToArray();

        using var hardwareEncryptor = CreateCore("hardware", encrypting: true);
        using var fallbackDecryptor = CreateCore("fallback", encrypting: false);
        hardwareEncryptor.Transform(data);
        fallbackDecryptor.Transform(data);
        Assert.Equal(plain, data);

        using var fallbackEncryptor = CreateCore("fallback", encrypting: true);
        using var hardwareDecryptor = CreateCore("hardware", encrypting: false);
        fallbackEncryptor.Transform(data);
        hardwareDecryptor.Transform(data);
        Assert.Equal(plain, data);
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Transform_ShouldAcceptEmptyBuffer(string core)
    {
        using var cipher = CreateCore(core, encrypting: true);
        cipher.Transform(Span<byte>.Empty);
    }

    [Fact]
    public void CreateEncryptor_ShouldThrow_OnWrongSecretLength()
    {
        Assert.Throws<ArgumentException>(() => PacketCipher.CreateEncryptor(new byte[8]));
        Assert.Throws<ArgumentException>(() => PacketCipher.CreateDecryptor(new byte[17]));
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Decrypt_ShouldMatchReferenceAndSerial_OnRandomSplits(string core)
    {
        int seed = Random.Shared.Next();
        output.WriteLine($"seed={seed}");
        var random = new Random(seed);

        for (int round = 0; round < 200; round++)
        {
            int length = random.Next(0, 600);
            byte[] plain = new byte[length];
            random.NextBytes(plain);
            byte[] encrypted = ReferenceTransform(true, plain);

            byte[] pipelined = encrypted.ToArray();
            using (var cipher = CreateCore(core, encrypting: false))
            {
                int position = 0;
                while (position < pipelined.Length)
                {
                    int chunk = random.Next(4) switch
                    {
                        0 => random.Next(1, 4),
                        1 => 16 * random.Next(1, 4),
                        2 => random.Next(15, 19),
                        _ => random.Next(1, 200),
                    };
                    chunk = Math.Min(chunk, pipelined.Length - position);
                    cipher.Transform(pipelined.AsSpan(position, chunk));
                    position += chunk;
                }
            }

            byte[] serial = encrypted.ToArray();
            using (var cipher = CreateCore(core, encrypting: false))
            {
                for (int i = 0; i < serial.Length; i++)
                {
                    cipher.Transform(serial.AsSpan(i, 1));
                }
            }

            Assert.Equal(plain, pipelined);
            Assert.Equal(plain, serial);
        }
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Decrypt_ShouldCarryState_AcrossPipelinedAndSerialChunks(string core)
    {
        int[][] patterns =
        [
            [16, 16, 16],
            [15, 1, 16, 17, 15],
            [1, 15, 16, 33, 2, 14, 48],
            [20, 4, 12, 16, 7, 9, 32],
            [31, 1, 31, 1, 64],
        ];

        foreach (int[] pattern in patterns)
        {
            int total = pattern.Sum();
            byte[] plain = new byte[total];
            new Random(total).NextBytes(plain);
            byte[] data = ReferenceTransform(true, plain);

            using var cipher = CreateCore(core, encrypting: false);
            int position = 0;
            foreach (int chunk in pattern)
            {
                cipher.Transform(data.AsSpan(position, chunk));
                position += chunk;
            }

            Assert.Equal(plain, data);

            byte[] tail = ReferenceTransform(true, plain.Concat(new byte[40]).ToArray())[total..];
            cipher.Transform(tail);
            Assert.Equal(new byte[40], tail);
        }
    }

    [Theory]
    [MemberData(nameof(Cores))]
    public void Decrypt_ShouldMatchFallback_OnRandomSplits(string core)
    {
        int seed = Random.Shared.Next();
        output.WriteLine($"seed={seed}");
        var random = new Random(seed);

        byte[] plain = new byte[100_000];
        random.NextBytes(plain);
        byte[] encrypted = ReferenceTransform(true, plain);

        byte[] actual = encrypted.ToArray();
        byte[] expected = encrypted.ToArray();
        using var cipher = CreateCore(core, encrypting: false);
        using var fallback = CreateCore("fallback", encrypting: false);

        int position = 0;
        while (position < actual.Length)
        {
            int chunk = Math.Min(random.Next(1, 3000), actual.Length - position);
            cipher.Transform(actual.AsSpan(position, chunk));
            fallback.Transform(expected.AsSpan(position, chunk));
            position += chunk;
        }

        Assert.Equal(expected, actual);
        Assert.Equal(plain, actual);
    }
}
