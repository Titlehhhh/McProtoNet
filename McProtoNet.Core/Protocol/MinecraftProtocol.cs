
using Ionic.Zlib;
using McProtoNet.Core.IO;

using System.Net.Sockets;


namespace McProtoNet.Core.Protocol
{
    public sealed class MinecraftProtocol : IPacketProtocol
    {
        private const int ZERO_VARLENGTH = 1;//default(int).GetVarIntLength();
        private NetworkMinecraftStream netmcStream;

        private int _compressionThreshold;



        public MinecraftProtocol(NetworkMinecraftStream netmcStream)
        {
            this.netmcStream = netmcStream;

        }
        public MinecraftProtocol(NetworkStream networkStream)
        {
            netmcStream = new NetworkMinecraftStream(networkStream);
        }

        public MinecraftProtocol(Socket socket) : this(new NetworkStream(socket))
        {

        }
        public MinecraftProtocol(TcpClient tcpClient) : this(tcpClient.GetStream())
        {

        }



        #region Async
        [Obsolete("Этот метод не рекомендуется использовать, из-за соображений производительности. Используйте ReadNextPacket()")]
        public async Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken token)
        {
            ThrowIfDisposed();
            try
            {
                token.ThrowIfCancellationRequested();
                int len = await netmcStream.ReadVarIntAsync(token);
                token.ThrowIfCancellationRequested();
                // Console.WriteLine("len " + len);
                byte[] receivedata = new byte[len];
                await netmcStream.ReadAsync(receivedata.AsMemory(0, len), token);


                var dataStream = new MemoryStream(receivedata);

                if (_compressionThreshold > 0)
                {

                    int sizeUncompressed = dataStream.ReadVarInt();
                    if (sizeUncompressed != 0)
                    {
                        ZlibStream zlibStream = new ZlibStream(dataStream, CompressionMode.Decompress);
                        byte[] uncompressdata = new byte[sizeUncompressed];
                        zlibStream.Read(uncompressdata, 0, sizeUncompressed);
                        zlibStream.Close();
                        zlibStream.Dispose();
                        dataStream = new MemoryStream(uncompressdata);
                    }

                }

                int id = dataStream.ReadVarInt();

                return (id, dataStream);
            }
            catch
            {
                throw;
            }

        }
        [Obsolete("Этот метод не рекомендуется использовать, из-за соображений производительности. Используйте SendPacket()")]
        public async Task SendPacketAsync(Packet packet, int id, CancellationToken token = default)
        {
            ThrowIfDisposed();

            try
            {
                ArgumentNullException.ThrowIfNull(packet, nameof(packet));
                if (_compressionThreshold > 0)
                {
                    using (MemoryStream bufferStream = new MemoryStream())
                    {
                        IMinecraftPrimitiveWriter packetStream = new MinecraftPrimitiveWriter(bufferStream);
                        packetStream.WriteVarInt(id);
                        packet.Write(packetStream);

                        int to_Packetlength = (int)bufferStream.Length;

                        if (to_Packetlength >= _compressionThreshold)
                        {
                            await SendLongPacketAsync(bufferStream, to_Packetlength, token);
                        }
                        else
                        {
                            await SendShortPacketAsync(bufferStream, token);
                        }
                    }
                }
                else
                {
                    await SendPacketWithoutCompressionAsync(packet, id, token);
                }
            }
            catch
            {
                throw;
            }


        }

        private async Task SendPacketWithoutCompressionAsync(Packet packet, int id, CancellationToken token)
        {
            ThrowIfDisposed();
            using (MemoryStream bufferStream = new MemoryStream())
            {
                IMinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter(bufferStream);


                packet.Write(writer);


                int Packetlength = (int)bufferStream.Length;

                await netmcStream.Lock.WaitAsync(token);

                await netmcStream.WriteVarIntAsync(Packetlength + id.GetVarIntLength(), token);
                await netmcStream.WriteVarIntAsync(id, token);
                bufferStream.Position = 0;

                bufferStream.CopyTo(netmcStream);
                netmcStream.Lock.Release();
            }
        }

        private async Task SendLongPacketAsync(Stream packetStream, int to_Packetlength, CancellationToken token)
        {
            ThrowIfDisposed();
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(compressedStream, CompressionMode.Compress))
                {
                    packetStream.Position = 0;
                    packetStream.CopyTo(stream);
                }

                int fullSize = (int)packetStream.Length + to_Packetlength.GetVarIntLength();

                await netmcStream.Lock.WaitAsync(token);

                await netmcStream.WriteVarIntAsync(fullSize, token);
                await netmcStream.WriteVarIntAsync(to_Packetlength, token);
                compressedStream.Position = 0;
                compressedStream.CopyTo(netmcStream);

                netmcStream.Lock.Release();
            }
        }

        private async Task SendShortPacketAsync(Stream packetStream, CancellationToken token)
        {
            ThrowIfDisposed();
            int fullSize = (int)packetStream.Length + ZERO_VARLENGTH;
            await netmcStream.Lock.WaitAsync(token);
            await netmcStream.WriteVarIntAsync(fullSize, token);
            await netmcStream.WriteVarIntAsync(0, token);
            packetStream.Position = 0;
            packetStream.CopyTo(netmcStream);
            netmcStream.Lock.Release();
        }

        public void SwitchEncryption(byte[] privateKey)
        {
            ThrowIfDisposed();
            netmcStream.Lock.Wait();
            netmcStream.SwitchEncryption(privateKey);
            netmcStream.Lock.Release();
        }

        public void SwitchCompression(int threshold)
        {
            ThrowIfDisposed();
            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold));
            //netmcStream.Lock.Wait();
            _compressionThreshold = threshold;
            //netmcStream.Lock.Release();
        }
        #endregion

        #region Sync
        public void SendPacket(Packet packet, int id)
        {
            ThrowIfDisposed();
            if (_compressionThreshold > 0)
            {
                using (MemoryStream bufferStream = new MemoryStream())
                {
                    IMinecraftPrimitiveWriter packetStream = new MinecraftPrimitiveWriter(bufferStream);
                    packetStream.WriteVarInt(id);
                    packet.Write(packetStream);

                    int to_Packetlength = (int)bufferStream.Length;

                    if (to_Packetlength >= _compressionThreshold)
                    {
                        SendLongPacket(bufferStream, to_Packetlength);
                    }
                    else
                    {
                        SendShortPacket(bufferStream);
                    }
                }
            }
            else
            {
                SendPacketWithoutCompression(packet, id);
            }
        }

        private void SendPacketWithoutCompression(Packet packet, int id)
        {
            ThrowIfDisposed();
            using (MemoryStream bufferStream = new MemoryStream())
            {
                IMinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter(bufferStream);


                packet.Write(writer);


                int Packetlength = (int)bufferStream.Length;

                netmcStream.Lock.Wait();

                netmcStream.WriteVarInt(Packetlength + id.GetVarIntLength());
                netmcStream.WriteVarInt(id);
                netmcStream.Flush();
                bufferStream.Position = 0;

                bufferStream.CopyTo(netmcStream);
                netmcStream.Flush();
                netmcStream.Lock.Release();
            }
        }

        private void SendShortPacket(MemoryStream packetStream)
        {
            ThrowIfDisposed();
            int fullSize = (int)packetStream.Length + ZERO_VARLENGTH;
            netmcStream.Lock.Wait();
            netmcStream.WriteVarInt(fullSize);
            netmcStream.WriteVarInt(0);
            packetStream.Position = 0;
            netmcStream.Flush();
            packetStream.CopyTo(netmcStream);
            netmcStream.Flush();
            netmcStream.Lock.Release();
        }

        private void SendLongPacket(MemoryStream packetStream, int to_Packetlength)
        {
            ThrowIfDisposed();
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(compressedStream, CompressionMode.Compress))
                {
                    packetStream.Position = 0;
                    packetStream.CopyTo(stream);
                }

                int fullSize = (int)packetStream.Length + to_Packetlength.GetVarIntLength();

                netmcStream.Lock.Wait();

                netmcStream.WriteVarInt(fullSize);
                netmcStream.WriteVarInt(to_Packetlength);
                compressedStream.Position = 0;
                compressedStream.CopyTo(netmcStream);
                netmcStream.Flush();
                netmcStream.Lock.Release();
            }
        }

        public (int, MemoryStream) ReadNextPacket()
        {
            ThrowIfDisposed();
            int len = netmcStream.ReadVarInt();

            MemoryStream dataStream = new MemoryStream();
            byte[] buffer = new byte[len];
            int read = 0;
            while (len > 0)
            {
                read = netmcStream.Read(buffer, 0, len);
                len -= read;
                dataStream.Write(buffer, 0, read);
            }
            dataStream.Position = 0;





            if (_compressionThreshold > 0)
            {

                int sizeUncompressed = dataStream.ReadVarInt();
                if (sizeUncompressed != 0)
                {
                    ZlibStream zlibStream = new ZlibStream(dataStream, CompressionMode.Decompress);
                    byte[] uncompressdata = new byte[sizeUncompressed];
                    zlibStream.Read(uncompressdata, 0, sizeUncompressed);
                    zlibStream.Close();
                    zlibStream.Dispose();
                    dataStream = new MemoryStream(uncompressdata);
                }

            }

            int id = dataStream.ReadVarInt();

            return (id, dataStream);
        }
        #endregion

        public bool Available()
        {
            ThrowIfDisposed();
            return netmcStream.NetStream.DataAvailable;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MinecraftProtocol));
        }

        private bool _disposed = false;
        ~MinecraftProtocol()
        {
            Dispose(false);
        }
        #region DisposeSync
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {

            }

            this.netmcStream.Dispose();

            _disposed = true;
        }
        #endregion

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(true);
        }
        private async ValueTask DisposeAsyncCore()
        {
            if (netmcStream is not null)
            {
                await netmcStream.DisposeAsync().ConfigureAwait(false);
            }
            netmcStream = null;
        }

        public void SendPacket(byte[] data, int id)
        {
            throw new NotImplementedException();
            ThrowIfDisposed();

        }

        public void SendPacket(MemoryStream data, int id)
        {
            throw new NotImplementedException();
            ThrowIfDisposed();
            data.Position = 0;
            data.WriteVarInt(id);
        }

        public void SendPacket(Span<byte> data, int id)
        {
            throw new NotImplementedException();
            ThrowIfDisposed();

        }
    }


}
