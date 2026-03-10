using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

public class EncryptedPipeWriterTests
{
    private static readonly byte[] TestKey = "0123456789ABCDEF"u8.ToArray();

    private static IBufferedCipher CreateCipher(bool forEncryption)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(TestKey), TestKey));
        return cipher;
    }

    [Fact]
    public async Task Should_Write_And_Encrypt_Data()
    {
        // Arrange
        var basePipe = new Pipe();
        var writer = new EncryptedPipeWriter(basePipe.Writer);
        var encryptor = CreateCipher(true);
        writer.SwitchEncryption(encryptor);

        var plainText = "Hello Minecraft World!";
        byte[] inputData = Encoding.UTF8.GetBytes(plainText);

        // Act
        inputData.CopyTo(writer.GetMemory(inputData.Length));
        writer.Advance(inputData.Length);
        await writer.FlushAsync();

        basePipe.Writer.Complete();

        // Read raw (encrypted) data
        ReadResult encryptedResult = await basePipe.Reader.ReadAsync();
        byte[] encryptedData = encryptedResult.Buffer.ToArray();
        basePipe.Reader.AdvanceTo(encryptedResult.Buffer.End);

        // Assert — should not equal plaintext
        Assert.NotEqual(inputData, encryptedData);

        // Decrypt manually to verify
        var decryptor = CreateCipher(false);
        byte[] decrypted = new byte[decryptor.GetOutputSize(encryptedData.Length)];
        int len = decryptor.ProcessBytes(encryptedData, 0, encryptedData.Length, decrypted, 0);

        string result = Encoding.UTF8.GetString(decrypted, 0, len);
        Assert.Equal(plainText, result);
    }

    [Fact]
    public async Task Should_Handle_Flush_And_Complete()
    {
        // Arrange
        var basePipe = new Pipe();
        var writer = new EncryptedPipeWriter(basePipe.Writer);
        writer.SwitchEncryption(CreateCipher(true));

        var input = Encoding.UTF8.GetBytes("flush test");
        input.CopyTo(writer.GetMemory(input.Length));
        writer.Advance(input.Length);

        // Act
        var flushResult = await writer.FlushAsync();
        writer.Complete();

        // Assert
        Assert.False(flushResult.IsCanceled);
    }

    [Fact]
    public async Task Should_Work_Without_Encryption()
    {
        // Arrange
        var basePipe = new Pipe();
        var writer = new EncryptedPipeWriter(basePipe.Writer);

        byte[] plain = Encoding.UTF8.GetBytes("no encryption test");
        plain.CopyTo(writer.GetMemory(plain.Length));
        writer.Advance(plain.Length);

        // Act
        await writer.FlushAsync();
        writer.Complete();

        // Assert
        ReadResult read = await basePipe.Reader.ReadAsync();
        string result = Encoding.UTF8.GetString(read.Buffer.ToArray());
        Assert.Equal("no encryption test", result);
    }


    [Fact]
    public async Task Should_Encrypt_Multiple_Segments()
    {
        // Arrange
        var basePipe = new Pipe();
        var writer = new EncryptedPipeWriter(basePipe.Writer);
        writer.SwitchEncryption(CreateCipher(true));

        // Создаем несколько сегментов
        byte[][] segments =
        {
            Encoding.UTF8.GetBytes("segment1-"),
            Encoding.UTF8.GetBytes("segment2-"),
            Encoding.UTF8.GetBytes("segment3")
        };

        foreach (var seg in segments)
        {
            seg.CopyTo(writer.GetMemory(seg.Length));
            writer.Advance(seg.Length);
            await writer.FlushAsync();
        }

        writer.Complete();

        ReadResult encryptedResult = await basePipe.Reader.ReadAsync();
        byte[] encryptedData = encryptedResult.Buffer.ToArray();

        // Дешифруем обратно
        var decryptor = CreateCipher(false);
        byte[] decrypted = new byte[decryptor.GetOutputSize(encryptedData.Length)];
        int len = decryptor.ProcessBytes(encryptedData, 0, encryptedData.Length, decrypted, 0);

        string result = Encoding.UTF8.GetString(decrypted, 0, len);
        Assert.Equal("segment1-segment2-segment3", result);
    }
}