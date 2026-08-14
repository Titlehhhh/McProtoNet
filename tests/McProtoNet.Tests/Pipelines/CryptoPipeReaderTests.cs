using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using McProtoNet.Cryptography;
using McProtoNet.Net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace McProtoNet.Tests.Pipelines
{
    public class CryptoPipeReaderTests
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
            var reader = new CryptoPipeReader(pipe.Reader);

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
            var (encryptor, _, key, iv) = CreateAesCfb8Ciphers();

            var basePipe = new Pipe();
            var reader = new CryptoPipeReader(basePipe.Reader);
            reader.EnableEncryption(PacketCipher.CreateDecryptor(key));

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
            var reader = new CryptoPipeReader(pipe.Reader);

            var (_, _, key, _) = CreateAesCfb8Ciphers();
            reader.EnableEncryption(PacketCipher.CreateDecryptor(key));

            bool success = reader.TryRead(out var result);

            Assert.False(success);
            Assert.Equal(default, result);
        }

        [Fact]
        public async Task Complete_ShouldMarkBothPipesCompleted()
        {
            var pipe = new Pipe();
            var reader = new CryptoPipeReader(pipe.Reader);

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
            var reader = new CryptoPipeReader(pipe.Reader);

            var cts = new CancellationTokenSource();

            var readTask = reader.ReadAsync(cts.Token).AsTask();

            reader.CancelPendingRead();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => readTask);
        }

        [Fact]
        public async Task ReadAsync_ShouldDecryptStream_FedInTinyFragments()
        {
            var (encryptor, _, key, _) = CreateAesCfb8Ciphers();

            var pipe = new Pipe();
            var reader = new CryptoPipeReader(pipe.Reader);
            reader.EnableEncryption(PacketCipher.CreateDecryptor(key));

            byte[] plain = new byte[10_000];
            new Random(40).NextBytes(plain);
            byte[] encrypted = new byte[encryptor.GetOutputSize(plain.Length)];
            int encryptedLength = encryptor.ProcessBytes(plain, 0, plain.Length, encrypted, 0);

            var feeder = Task.Run(async () =>
            {
                var fragments = new Random(44);
                int position = 0;
                while (position < encryptedLength)
                {
                    int length = Math.Min(1 + fragments.Next(7), encryptedLength - position);
                    await pipe.Writer.WriteAsync(encrypted.AsMemory(position, length));
                    position += length;
                }

                pipe.Writer.Complete();
            });

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var collected = new MemoryStream();
            while (true)
            {
                ReadResult result = await reader.ReadAsync(timeout.Token);
                foreach (ReadOnlyMemory<byte> segment in result.Buffer)
                {
                    collected.Write(segment.Span);
                }

                reader.AdvanceTo(result.Buffer.End);
                if (result.IsCompleted)
                {
                    break;
                }
            }

            await feeder;
            Assert.Equal(plain, collected.ToArray());
        }

        [Fact]
        public async Task EnableEncryption_ShouldStartDecryption_AtBoundary()
        {
            var (encryptor, _, key, _) = CreateAesCfb8Ciphers();

            var pipe = new Pipe();
            var reader = new CryptoPipeReader(pipe.Reader);

            byte[] prefix = Encoding.UTF8.GetBytes("plain handshake");
            await pipe.Writer.WriteAsync(prefix);

            ReadResult plainResult = await reader.ReadAsync();
            Assert.Equal(prefix, plainResult.Buffer.ToArray());
            reader.AdvanceTo(plainResult.Buffer.End);

            reader.EnableEncryption(PacketCipher.CreateDecryptor(key));

            byte[] secret = Encoding.UTF8.GetBytes("secret payload");
            byte[] encrypted = new byte[encryptor.GetOutputSize(secret.Length)];
            int encryptedLength = encryptor.ProcessBytes(secret, 0, secret.Length, encrypted, 0);
            await pipe.Writer.WriteAsync(encrypted.AsMemory(0, encryptedLength));
            pipe.Writer.Complete();

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var collected = new MemoryStream();
            while (true)
            {
                ReadResult result = await reader.ReadAsync(timeout.Token);
                foreach (ReadOnlyMemory<byte> segment in result.Buffer)
                {
                    collected.Write(segment.Span);
                }

                reader.AdvanceTo(result.Buffer.End);
                if (result.IsCompleted)
                {
                    break;
                }
            }

            Assert.Equal(secret, collected.ToArray());
        }

        [Fact]
        public void EnableEncryption_ShouldThrow_WhenAlreadyEnabled()
        {
            var (_, _, key, _) = CreateAesCfb8Ciphers();
            var pipe = new Pipe();
            var reader = new CryptoPipeReader(pipe.Reader);
            reader.EnableEncryption(PacketCipher.CreateDecryptor(key));

            Assert.Throws<InvalidOperationException>(
                () => reader.EnableEncryption(PacketCipher.CreateDecryptor(key)));
        }

        [Fact]
        public void EnableEncryption_ShouldThrow_OnWrongSecretLength()
        {
            var pipe = new Pipe();
            var reader = new CryptoPipeReader(pipe.Reader);

            Assert.Throws<ArgumentException>(() => reader.EnableEncryption("12345678"u8));
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            var (_, _, key, _) = CreateAesCfb8Ciphers();
            var pipe = new Pipe();
            var reader = new CryptoPipeReader(pipe.Reader);
            reader.EnableEncryption(PacketCipher.CreateDecryptor(key));

            reader.Dispose();
            reader.Dispose();
        }
    }
}
