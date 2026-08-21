using McProtoNet.Transport.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Cryptography;

public class AesStreamTests
{
    private static readonly byte[] TestKey = "0123456789ABCDEF"u8.ToArray();

    private static IBufferedCipher CreateReferenceCipher(bool forEncryption)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(TestKey), TestKey));
        return cipher;
    }

    private static byte[] ReferenceTransform(bool forEncryption, byte[] data)
    {
        var cipher = CreateReferenceCipher(forEncryption);
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

    [Fact]
    public void Write_ShouldEncryptChunkedStream()
    {
        byte[] plain = RandomBytes(40_000);

        using var backing = new MemoryStream();
        using (var stream = new AesStream(backing, leaveOpen: true))
        {
            stream.EnableEncryption(TestKey);

            var chunks = new Random(41);
            int position = 0;
            while (position < plain.Length)
            {
                int length = Math.Min(1 + chunks.Next(5000), plain.Length - position);
                stream.Write(plain.AsSpan(position, length));
                position += length;
            }
        }

        Assert.Equal(ReferenceTransform(true, plain), backing.ToArray());
    }

    [Fact]
    public async Task WriteAsync_ShouldEncryptChunkedStream()
    {
        byte[] plain = RandomBytes(40_000);

        var backing = new MemoryStream();
        await using (var stream = new AesStream(backing, leaveOpen: true))
        {
            stream.EnableEncryption(TestKey);

            var chunks = new Random(41);
            int position = 0;
            while (position < plain.Length)
            {
                int length = Math.Min(1 + chunks.Next(5000), plain.Length - position);
                await stream.WriteAsync(plain.AsMemory(position, length));
                position += length;
            }
        }

        Assert.Equal(ReferenceTransform(true, plain), backing.ToArray());
    }

    [Fact]
    public void Read_ShouldDecryptChunkedStream()
    {
        byte[] plain = RandomBytes(40_000);
        byte[] encrypted = ReferenceTransform(true, plain);

        using var stream = new AesStream(new MemoryStream(encrypted));
        stream.EnableEncryption(TestKey);

        byte[] decrypted = new byte[plain.Length];
        var chunks = new Random(42);
        int position = 0;
        while (position < decrypted.Length)
        {
            int wanted = Math.Min(1 + chunks.Next(5000), decrypted.Length - position);
            int read = stream.Read(decrypted.AsSpan(position, wanted));
            Assert.True(read > 0);
            position += read;
        }

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public async Task ReadAsync_ShouldDecryptChunkedStream()
    {
        byte[] plain = RandomBytes(40_000);
        byte[] encrypted = ReferenceTransform(true, plain);

        await using var stream = new AesStream(new MemoryStream(encrypted));
        stream.EnableEncryption(TestKey);

        byte[] decrypted = new byte[plain.Length];
        var chunks = new Random(42);
        int position = 0;
        while (position < decrypted.Length)
        {
            int wanted = Math.Min(1 + chunks.Next(5000), decrypted.Length - position);
            int read = await stream.ReadAsync(decrypted.AsMemory(position, wanted));
            Assert.True(read > 0);
            position += read;
        }

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void RoundTrip_ShouldSurviveTwoStreams()
    {
        byte[] plain = RandomBytes(20_000);

        using var backing = new MemoryStream();
        using (var encryptingStream = new AesStream(backing, leaveOpen: true))
        {
            encryptingStream.EnableEncryption(TestKey);
            encryptingStream.Write(plain);
        }

        backing.Position = 0;
        using var decryptingStream = new AesStream(backing, leaveOpen: true);
        decryptingStream.EnableEncryption(TestKey);

        byte[] decrypted = new byte[plain.Length];
        decryptingStream.ReadExactly(decrypted);

        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void Stream_ShouldPassThrough_WithoutEncryption()
    {
        byte[] plain = RandomBytes(1000);

        using var backing = new MemoryStream();
        using (var stream = new AesStream(backing, leaveOpen: true))
        {
            stream.Write(plain);
        }

        Assert.Equal(plain, backing.ToArray());
    }

    [Fact]
    public void EnableEncryption_ShouldThrow_WhenAlreadyEnabled()
    {
        using var stream = new AesStream(new MemoryStream());
        stream.EnableEncryption(TestKey);

        Assert.Throws<InvalidOperationException>(() => stream.EnableEncryption(TestKey));
    }

    [Fact]
    public void SwitchEncryption_ShouldKeepCompatibilityDoor()
    {
        using var backing = new MemoryStream();
        using var stream = new AesStream(backing, leaveOpen: true);
        stream.SwitchEncryption(TestKey);

        Assert.True(stream.EncryptionEnabled);

        byte[] plain = RandomBytes(100);
        stream.Write(plain);
        Assert.Equal(ReferenceTransform(true, plain), backing.ToArray());
    }
}
