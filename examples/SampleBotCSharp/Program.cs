using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Security.Cryptography;
using McProtoNet.Protocol;
using SampleBotCSharp;
using System.Security.Cryptography;
using McProtoNet.Serialization;

namespace Obsidian.Net;

public sealed class AesCfbBlockCipher : IDisposable
{
    private const int BlockSize = 16;

    private readonly Aes aes;
    private readonly ICryptoTransform transform;

    private readonly byte[] iv;
    private readonly byte[] block;

    public AesCfbBlockCipher(byte[] key)
    {
        if (key.Length != 16)
            throw new ArgumentException("Key must be 16 bytes long");
        this.iv = new byte[key.Length];

        key.AsSpan().CopyTo(this.iv.AsSpan());

        this.block = new byte[BlockSize];

        this.aes = Aes.Create();
        this.aes.Mode = CipherMode.ECB;
        this.aes.Padding = PaddingMode.None;
        this.aes.FeedbackSize = 8;

        this.aes.Key = key;
        this.aes.IV = iv;

        this.transform = aes.CreateEncryptor();
    }

    public Span<byte> Encrypt(ReadOnlySpan<byte> buffer)
    {
        var output = new byte[buffer.Length];
        for (int i = 0; i < output.Length; i++)
        {
            this.transform.TransformBlock(this.iv, 0, BlockSize, this.block, 0);

            var cipherByte = (byte)(buffer[i] ^ this.block[0]);

            Buffer.BlockCopy(this.iv, 1, this.iv, 0, 15);

            this.iv[15] = cipherByte;

            output[i] = cipherByte;
        }

        return output;
    }

    public Span<byte> Encrypt(byte[] buffer, int offset, int count)
    {
        var output = new byte[count];
        for (int i = 0; i < count; i++)
        {
            var plainByte = buffer[offset + i];

            this.transform.TransformBlock(this.iv, 0, BlockSize, this.block, 0);

            var cipherByte = (byte)(plainByte ^ this.block[0]);

            Buffer.BlockCopy(this.iv, 1, this.iv, 0, 15);

            this.iv[15] = cipherByte;
            output[i] = cipherByte;
        }

        return output;
    }

    public byte[] Decrypt2(ReadOnlySpan<byte> input)
    {
        if (input.Length == 0) return Array.Empty<byte>();

        byte[] output = new byte[input.Length];

        bool ok = aes.TryDecryptCfb(
            input,
            this.iv, // current IV (mutable span/array)
            output,
            out int bytesWritten,
            paddingMode: PaddingMode.None,
            feedbackSizeInBits: 8);

        if (!ok) throw new CryptographicException("TryDecryptCfb failed.");
        if (bytesWritten != input.Length)
        {
            Environment.FailFast("asdasd");
            // trim if needed
            var trimmed = new byte[bytesWritten];
            Array.Copy(output, 0, trimmed, 0, bytesWritten);
            UpdateIvForCfb8(this.iv.AsSpan(), input.Slice(0, bytesWritten));
            return trimmed;
        }

        // Обновляем IV на основе ciphertext (входа)
        UpdateIvForCfb8(this.iv.AsSpan(), input);
        return output;
    }

    private static void UpdateIvForCfb8(Span<byte> iv, ReadOnlySpan<byte> ciphertext)
    {
        int block = iv.Length; // обычно 16
        int n = ciphertext.Length;
        if (n == 0) return;

        if (n >= block)
        {
            // copy last `block` bytes of ciphertext into iv
            ciphertext.Slice(n - block, block).CopyTo(iv);
            return;
        }

        // n < block: shift iv left by n bytes and append ciphertext at the end
        // iv[0..block-n-1] = old iv[n..block-1]
        iv.Slice(n, block - n).CopyTo(iv.Slice(0, block - n));
        // append ciphertext to the tail
        ciphertext.CopyTo(iv.Slice(block - n, n));
    }

    public byte[] Decrypt(ReadOnlySpan<byte> buffer, int offset, int count)
    {
        var output = new byte[count];
        for (int i = 0; i < count; i++)
        {
            var cipherByte = buffer[offset + i];

            this.transform.TransformBlock(this.iv, 0, BlockSize, this.block, 0);
            var plainByte = (byte)(cipherByte ^ this.block[0]);

            Buffer.BlockCopy(this.iv, 1, this.iv, 0, 15);
            this.iv[15] = cipherByte;

            output[i] = plainByte;
        }

        return output;
    }

    public void Dispose()
    {
        this.transform.Dispose();

        this.aes.Dispose();
    }
}

class Program
{
    static byte[] FromVarInt(int val)
    {
        byte[] buff = new byte[5];
        var len = val.GetVarIntLength(buff.AsSpan());
        return buff.AsSpan(0, len).ToArray();
    }

    static byte[] ToBytes(int val)
    {
        byte[] buff = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buff, val);
        return buff;
    }

    static void Test()
    {
        for (int i = 0; i < 50_000; i+=100)
        {
            byte[] varIntBytes = FromVarInt(i);
            byte[] intBytes = ToBytes(i);

            Console.Write("VarInt({0}): ", i);
            foreach (var b in varIntBytes)
                Console.Write("{0:X2} ", b);

            Console.Write("Bytes({0}): ", i);
            foreach (var b in intBytes)
                Console.Write("{0:X2} ", b);
            Console.WriteLine();
        }

        return;

        byte[] originalData = new byte[120];
        Random rng = new(42);
        rng.NextBytes(originalData);

        using Aes aes = Aes.Create();
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.GenerateKey();
        aes.GenerateIV();
        byte[] key = aes.Key;
        AesCfbBlockCipher encryptor = new(key);
        AesCfbBlockCipher decryptor = new(key);


        int pos = 0;
        List<byte[]> encryptedChunks = new List<byte[]>();
        List<int> chunkSizes = new List<int>();

        while (pos < originalData.Length)
        {
            int remaining = originalData.Length - pos;
            // размер куска 1..30 (или оставшееся)
            int chunkSize = Math.Min(remaining, rng.Next(1, 31));
            chunkSizes.Add(chunkSize);

            // используем overload Encrypt(byte[] buffer, int offset, int count)
            var encSpan = encryptor.Encrypt(originalData, pos, chunkSize);

            // копируем Span в отдельный массив (чтобы сохранить кусок)
            byte[] encChunk = new byte[encSpan.Length];
            encSpan.CopyTo(encChunk);

            encryptedChunks.Add(encChunk);

            pos += chunkSize;
        }


        byte[] allData = encryptedChunks.SelectMany(x => x).ToArray();


        var decryptedBlocks = allData
            .Chunk(37)
            .Select(x => decryptor.Decrypt2(x))
            .ToArray();

        byte[] allData2 = decryptedBlocks.SelectMany(x => x).ToArray();

        Console.WriteLine(originalData.SequenceEqual(allData2));
    }


    public static async Task Main(string[] args)
    {
        Test();

        return;
        Bot bot = new Bot(MinecraftVersion.V1_21_4, "title-kde");

        await bot.Start();

        await Task.Delay(-1);
    }
}