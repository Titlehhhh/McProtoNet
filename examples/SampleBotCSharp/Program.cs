using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.Intrinsics;
using McProtoNet.Protocol;
using SampleBotCSharp;
using System.Security.Cryptography;
using DotNext.Buffers;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Obsidian.Net;

class Program
{
    private static byte[] privatekey = Enumerable.Range(1, 16).Select(i => (byte)i).ToArray();

    private static List<byte[]> GetTestEncrypted(byte[] data)
    {
        var cryptor = CipherUtilities.GetCipher("AES/CFB8/NoPadding");

        cryptor.Init(true,
            new ParametersWithIV(
                new KeyParameter(privatekey), privatekey, 0, 16));

        MemoryStream ms = new();

        var cipher = new CipherStream(ms, null, cryptor);

        cipher.Write(data);
        //cipher.Dispose();

        ms.Position = 0;
        var arr = ms.ToArray();

        return arr.Chunk(50).ToList();
    }

    public static async Task Main(string[] args)
    {
        var random = new Random(55);

        var testData = new byte[560];

        random.NextBytes(testData);

        var encrypted = GetTestEncrypted(testData);

        var pipe = new Pipe(
            new PipeOptions(
                writerScheduler: PipeScheduler.Inline,
                readerScheduler: PipeScheduler.Inline));

        var decryptor = CipherUtilities.GetCipher("AES/CFB8/NoPadding");

        decryptor.Init(false,
            new ParametersWithIV(
                new KeyParameter(privatekey), privatekey, 0, 16));


        foreach (var segment in encrypted)
        {
            var gg = pipe.Writer.GetSpan(20);

            ReadOnlySpan<byte> src = segment.AsSpan();

            int a = decryptor.ProcessBytes(src, gg);
            if (a >= segment.Length)
            {
                pipe.Writer.Advance(a);
            }
            else
            {

            }
            
        }

        var test = decryptor.DoFinal();
        
        var vTask = pipe.Writer.FlushAsync();

        Console.WriteLine($"IsCompleted: {vTask.IsCompleted}");

        Console.WriteLine(pipe.Writer.UnflushedBytes);

        return;
        Bot bot = new Bot(MinecraftVersion.V1_21_4, "title-kde");

        await bot.Start();

        await Task.Delay(-1);
    }
}