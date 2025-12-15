using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Infrastructure;

public static class Crypto
{
    private static readonly byte[] Key = "ABCDEFGHIJKLMGASDASDGSGSDF"u8[..16].ToArray();
    
    public static IBufferedCipher CreateCFB(bool forEncrypt)
    {
        var cipher = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
        cipher.Init(forEncrypt, new ParametersWithIV(new KeyParameter(Key), Key));
        return cipher;
    }
}