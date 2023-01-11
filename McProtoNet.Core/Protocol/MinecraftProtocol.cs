
using Ionic.Zlib;
using McProtoNet.Core.IO;
using System.IO;
using System.Net.Sockets;

namespace McProtoNet.Core.Protocol
{
    public sealed class MinecraftProtocol : IMinecraftProtocol
    {
        private const int ZERO_VARLENGTH = 1;//default(int).GetVarIntLength();
        private NetworkMinecraftStream netmcStream;

        private int _compressionThreshold;



        public MinecraftProtocol(NetworkMinecraftStream netmcStream)
        {
            this.netmcStream = netmcStream;
        }
        public MinecraftProtocol(NetworkStream networkStream) : this(new NetworkMinecraftStream(networkStream))
        { }

        public MinecraftProtocol(Socket socket) : this(new NetworkStream(socket))
        { }
        public MinecraftProtocol(TcpClient tcpClient) : this(tcpClient.GetStream())
        { }



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
        #region Async
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
        public async Task SendPacketAsync(MemoryStream packet, int id, CancellationToken token = default)
        {
            using (MemoryStream bufferStream = new MemoryStream())
            {
                await bufferStream.WriteVarIntAsync(id, token);
                packet.Position = 0;
                await packet.CopyToAsync(bufferStream, token);

                if (_compressionThreshold > 0)
                {
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
                else
                {
                    await SendPacketWithoutCompressionAsync(bufferStream, id, token);
                }
            }
        }

        private async Task SendPacketWithoutCompressionAsync(MemoryStream packet, int id, CancellationToken token)
        {
            ThrowIfDisposed();
            await netmcStream.Lock.WaitAsync(token);
            int Packetlength = (int)packet.Length;

            await netmcStream.WriteVarIntAsync(Packetlength, token);

            packet.Position = 0;
            await packet.CopyToAsync(netmcStream, token);
            netmcStream.Lock.Release();

        }

        private async Task SendLongPacketAsync(Stream packetStream, int to_Packetlength, CancellationToken token)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(compressedStream, CompressionMode.Compress))
                {
                    packetStream.Position = 0;
                    await packetStream.CopyToAsync(stream, token);
                }

                int fullSize = (int)packetStream.Length + to_Packetlength.GetVarIntLength();

                await netmcStream.Lock.WaitAsync(token);

                await netmcStream.WriteVarIntAsync(fullSize, token);
                await netmcStream.WriteVarIntAsync(to_Packetlength, token);

                compressedStream.Position = 0;
                await compressedStream.CopyToAsync(netmcStream, token);

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


        #endregion
        #region Sync
        public void SendPacket(MemoryStream packet, int id)
        {

            ThrowIfDisposed();

            if (_compressionThreshold > 0)
            {
                using (MemoryStream bufferStream = new MemoryStream())
                {
                    packet.Position = 0;
                    bufferStream.WriteVarInt(id);
                    packet.CopyTo(bufferStream);

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

        private void SendPacketWithoutCompression(MemoryStream packet, int id)
        {
            ThrowIfDisposed();
            // packet.Write(writer);
            int Packetlength = (int)packet.Length;
            netmcStream.Lock.Wait();

            //Записываем длину всего пакета
            netmcStream.WriteVarInt(Packetlength + id.GetVarIntLength());
            //Записываем ID пакета
            netmcStream.WriteVarInt(id);
            netmcStream.Flush();

            packet.Position = 0;
            //Все данные пакета перекидваем в интернет
            packet.CopyTo(netmcStream);

            netmcStream.Flush();

            netmcStream.Lock.Release();

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
            int read;
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
        ~MinecraftProtocol()
        {
            Dispose();
        }
        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            if (netmcStream is not null)
            {
                try
                {
                    netmcStream.Close();
                }
                catch { }
                netmcStream.Dispose();

                netmcStream = null;
            }


            GC.SuppressFinalize(this);
        }


    }


}
