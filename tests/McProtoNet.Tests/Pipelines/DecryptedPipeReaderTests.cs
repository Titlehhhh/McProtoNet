using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines
{
    public class DecryptedPipeReaderTests
    {
        private static (IBufferedCipher encryptor, IBufferedCipher decryptor, byte[] key, byte[] iv)
            CreateAesCfb8Ciphers()
        {
            byte[] key = new byte[16];
            new SecureRandom().NextBytes(key);
            

            var encryptor = CipherUtilities.GetCipher("AES/CFB8/NoPadding");
            var decryptor = CipherUtilities.GetCipher("AES/CFB8/NoPadding");

            encryptor.Init(true, new ParametersWithIV(new KeyParameter(key), key));
            decryptor.Init(false, new ParametersWithIV(new KeyParameter(key), key));

            return (encryptor, decryptor, key, key);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnPlainData_WhenNotEncrypted()
        {
            // Arrange
            var pipe = new Pipe();
            var reader = new DecryptedPipeReader(pipe.Reader);

            string testMessage = "Hello world!";
            byte[] testData = Encoding.UTF8.GetBytes(testMessage);

            await pipe.Writer.WriteAsync(testData);
            await pipe.Writer.CompleteAsync();

            // Act
            var result = await reader.ReadAsync();
            string actual = Encoding.UTF8.GetString(result.Buffer.ToArray());

            // Assert
            Assert.Equal(testMessage, actual);

            reader.AdvanceTo(result.Buffer.End);
            reader.Complete();
        }

        [Fact]
        public async Task ReadAsync_ShouldDecryptData_WhenEncrypted()
        {
            // Arrange
            var (encryptor, decryptor, key, iv) = CreateAesCfb8Ciphers();

            var basePipe = new Pipe();
            var reader = new DecryptedPipeReader(basePipe.Reader);
            reader.SwitchEncryption(decryptor);

            string plainText = "Hello AES CFB8!";
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] encrypted = new byte[encryptor.GetOutputSize(plainBytes.Length)];
            int len = encryptor.ProcessBytes(plainBytes, 0, plainBytes.Length, encrypted, 0);
            len += encryptor.DoFinal(encrypted, len);

            await basePipe.Writer.WriteAsync(encrypted.AsMemory(0, len));
            await basePipe.Writer.CompleteAsync();

            // Act
            ReadResult rr = await reader.ReadAsync();
            string decrypted = Encoding.UTF8.GetString(rr.Buffer.ToArray());

            // Assert
            Assert.Equal(plainText, decrypted);

            reader.AdvanceTo(rr.Buffer.End);
            reader.Complete();
        }

        [Fact]
        public void TryRead_ShouldReturnFalse_WhenEncryptedAndNoData()
        {
            var pipe = new Pipe();
            var reader = new DecryptedPipeReader(pipe.Reader);

            var (_, decryptor, _, _) = CreateAesCfb8Ciphers();
            reader.SwitchEncryption(decryptor);

            bool success = reader.TryRead(out var result);

            Assert.False(success);
            Assert.Equal(default, result);
        }

        [Fact]
        public async Task Complete_ShouldMarkBothPipesCompleted()
        {
            var pipe = new Pipe();
            var reader = new DecryptedPipeReader(pipe.Reader);

            reader.Complete();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await pipe.Reader.ReadAsync();
            });
        }

        [Fact]
        public async Task CancelPendingRead_ShouldCancelCurrentRead()
        {
            var pipe = new Pipe();
            var reader = new DecryptedPipeReader(pipe.Reader);

            var cts = new CancellationTokenSource();

            var readTask = reader.ReadAsync(cts.Token).AsTask();

            reader.CancelPendingRead();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => readTask);
        }
    }
}
