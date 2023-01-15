﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Buffers.Binary;
using System.Net.Sockets;

namespace McProtoNet.Core.Protocol
{

    public sealed class NetworkMinecraftStream : Stream, IDisposable
    {
        public NetworkStream NetStream => (NetworkStream)BaseStream;

        public bool EncryptionEnabled { get; private set; } = false;

        internal Stream BaseStream;

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

        public NetworkMinecraftStream(NetworkStream networkStream)
        {
            ArgumentNullException.ThrowIfNull(networkStream, nameof(networkStream));


            this.BaseStream = networkStream;
        }
        public NetworkMinecraftStream(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));


            this.BaseStream = stream;
        }


        public int ReadVarInt()
        {
            return BaseStream.ReadVarInt();
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {

                read = ReadUnsignedByte();

                int value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);
            return result;
        }
        public int ReadVarInt(out byte len)
        {


            int numRead = 0;
            int result = 0;
            byte read;
            do
            {

                read = ReadUnsignedByte();

                int value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);
            len = (byte)numRead;
            return result;
        }

        public async ValueTask<int> ReadVarIntAsync(CancellationToken token = default)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                token.ThrowIfCancellationRequested();
                read = await this.ReadUnsignedByteAsync(token).ConfigureAwait(false);

                int value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;

        }
        public async ValueTask<(int, int)> ReadVarIntAndLenAsync(CancellationToken token = default)
        {
            int numRead = 0;
            int result = 0;
            byte read;
            do
            {
                token.ThrowIfCancellationRequested();
                read = await this.ReadUnsignedByteAsync(token).ConfigureAwait(false);

                int value = read & 0b01111111;
                result |= value << (7 * numRead);

                numRead++;
                if (numRead > 5)
                {
                    throw new InvalidOperationException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return (result, numRead);

        }




        #region Приватные

        private byte ReadUnsignedByte()
        {
            int b = ReadByte();
            if (b == -1)
                throw new InvalidOperationException("Stream end");
            return (byte)b;
        }
        private void WriteUnsignedByte(byte val)
        {
            WriteByte(val);
        }

        private async ValueTask<byte> ReadUnsignedByteAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var buffer = new byte[1];
            await this.ReadAsync(buffer, token).ConfigureAwait(false);
            return buffer[0];

        }
        private async ValueTask WriteUnsignedByteAsync(byte value, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await WriteAsync(new[] { value }, token).ConfigureAwait(false);
        }
        private IBufferedCipher EncryptCipher { get; set; }
        private IBufferedCipher DecryptCipher { get; set; }
        #endregion
        /// <summary>
        /// Включает AES шифрование
        /// </summary>
        /// <param name="privatekey">Ключ</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SwitchEncryption(byte[] privatekey)
        {
            if (EncryptionEnabled)
            {
                throw new InvalidOperationException("Шифрование уже включено");
            }

            EncryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
            EncryptCipher.Init(true, new ParametersWithIV(new KeyParameter(privatekey), privatekey, 0, 16));

            DecryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
            DecryptCipher.Init(false, new ParametersWithIV(new KeyParameter(privatekey), privatekey, 0, 16));

            this.BaseStream = new CipherStream(BaseStream, DecryptCipher, EncryptCipher);
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return BaseStream.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }
        /// <summary>
        /// asd
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return BaseStream.ReadAsync(buffer, cancellationToken);
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return BaseStream.WriteAsync(buffer, cancellationToken);
        }
        protected override void Dispose(bool disposing)
        {
            BaseStream.Dispose();
            base.Dispose(disposing);
        }

    }
}
