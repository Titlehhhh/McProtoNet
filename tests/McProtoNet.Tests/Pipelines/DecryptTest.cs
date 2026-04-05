using System.Buffers;
using System.IO.Pipelines;
using McProtoNet.Net;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines;

public class DecryptTest
{
    private readonly Random random = new Random(55);

    private static readonly byte[] PrivateKey = Enumerable.Range(0, 16).Select(i => (byte)i).ToArray();

    private byte[] GenTestBuffer(int len)
    {
        byte[] b = new byte[len];
        random.NextBytes(b);
        return b;
    }

    private byte[] Crypt(byte[] data)
    {
        var result = new byte[data.Length];
        var bufferedCipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        bufferedCipher.Init(true,
            new ParametersWithIV(
                new KeyParameter(PrivateKey), 
                PrivateKey, 0, 16));
        
        bufferedCipher.ProcessBytes(data.AsSpan(), result.AsSpan());
        return result;
    }

    [Theory]
    [InlineData(500)]
    public async Task Test1(int len)
    {
        var buff = GenTestBuffer(len);

        var crypted = Crypt(buff);
        
        var ms = new MemoryStream(crypted);
        
        Assert.Equal(0, ms.Position);

        var pipeReader = PipeReader.Create(ms);

        var tryR = pipeReader.TryRead(out _);
        
        Assert.False(tryR);
        
        var decryptor = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        decryptor.Init(false,
            new ParametersWithIV(
                new KeyParameter(PrivateKey), 
                PrivateKey, 0, 16));

        var decryptReader = new DecryptedPipeReader(pipeReader);
        
        decryptReader.SwitchEncryption(decryptor);

        var tryRead = decryptReader.TryRead(out _);
        
        Assert.False(tryRead, $"TryRead is False");


        var res = await decryptReader.ReadAtLeastAsync(len);
        
        Assert.False(res.IsCanceled);
        Assert.False(res.IsCompleted);
        
        Assert.Equal(res.Buffer.Length, len);


        var resArr = res.Buffer.ToArray();
        
        Assert.Equal(buff, resArr);
    }
}