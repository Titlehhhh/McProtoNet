
//using Ionic.Zlib;
using McProtoNet.Core.IO;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace McProtoNet.Core.Protocol
{
    public sealed class MinecraftProtocol : IMinecraftProtocol
    {
        private const int ZERO_VARLENGTH = 1;//default(int).GetVarIntLength();
        private readonly byte[] ZERO_VARINT = { 0 };
        private NetworkMinecraftStream netmcStream;

        private int _compressionThreshold;


        private readonly bool _disposedStream;
        public MinecraftProtocol(NetworkMinecraftStream netmcStream, bool disposedStream)
        {
            this.netmcStream = netmcStream;
            _disposedStream = disposedStream;
        }
        public MinecraftProtocol(NetworkStream networkStream, bool disposedStream)
            : this(new NetworkMinecraftStream(networkStream), disposedStream)
        { }

        public MinecraftProtocol(Socket socket, bool disposedStream)
            : this(new NetworkStream(socket), disposedStream)
        { }
        public MinecraftProtocol(TcpClient tcpClient, bool disposedStream)
            : this(tcpClient.GetStream(), disposedStream)
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
                int len = await netmcStream.ReadVarIntAsync(token).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();
                // Console.WriteLine("len " + len);
                byte[] receivedata = new byte[len];
                await netmcStream.ReadAsync(receivedata.AsMemory(0, len), token).ConfigureAwait(false);


                var dataStream = new MemoryStream(receivedata);

                if (_compressionThreshold > 0)
                {

                    int sizeUncompressed = dataStream.ReadVarInt();
                    if (sizeUncompressed != 0)
                    {
                        ZLibStream zlibStream = new ZLibStream(dataStream, CompressionMode.Decompress);
                        byte[] uncompressdata = new byte[sizeUncompressed];
                        await zlibStream.ReadAsync(uncompressdata, 0, sizeUncompressed, token).ConfigureAwait(false);
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
                await bufferStream.WriteVarIntAsync(id, token).ConfigureAwait(false);
                packet.Position = 0;
                await packet.CopyToAsync(bufferStream, token).ConfigureAwait(false);

                if (_compressionThreshold > 0)
                {
                    int to_Packetlength = (int)bufferStream.Length;

                    if (to_Packetlength >= _compressionThreshold)
                    {
                        await SendLongPacketAsync(bufferStream, to_Packetlength, token).ConfigureAwait(false);
                    }
                    else
                    {
                        await SendShortPacketAsync(bufferStream, token).ConfigureAwait(false);
                    }

                }
                else
                {
                    await SendPacketWithoutCompressionAsync(bufferStream, id, token).ConfigureAwait(false);
                }
            }
            await netmcStream.FlushAsync(token).ConfigureAwait(false);
        }

        private async Task SendPacketWithoutCompressionAsync(MemoryStream packet, int id, CancellationToken token)
        {
            ThrowIfDisposed();
            await netmcStream.Lock.WaitAsync(token).ConfigureAwait(false);
            int Packetlength = (int)packet.Length;

            await netmcStream.WriteVarIntAsync(Packetlength, token).ConfigureAwait(false);

            packet.Position = 0;
            await packet.CopyToAsync(netmcStream, token).ConfigureAwait(false);
            netmcStream.Lock.Release();

        }

        private async Task SendLongPacketAsync(Stream packetStream, int to_Packetlength, CancellationToken token)
        {
            using (MemoryStream compressedStream = new MemoryStream())
            {
                using (ZLibStream stream = new ZLibStream(compressedStream, CompressionMode.Compress))
                {
                    packetStream.Position = 0;
                    await packetStream.CopyToAsync(stream, token).ConfigureAwait(false);
                }

                int fullSize = (int)packetStream.Length + to_Packetlength.GetVarIntLength();

                await netmcStream.Lock.WaitAsync(token).ConfigureAwait(false);

                await netmcStream.WriteVarIntAsync(fullSize, token).ConfigureAwait(false);
                await netmcStream.WriteVarIntAsync(to_Packetlength, token).ConfigureAwait(false);

                compressedStream.Position = 0;
                await compressedStream.CopyToAsync(netmcStream, token).ConfigureAwait(false);

                netmcStream.Lock.Release();
            }
        }

        private async Task SendShortPacketAsync(Stream packetStream, CancellationToken token)
        {
            ThrowIfDisposed();
            int fullSize = (int)packetStream.Length + ZERO_VARLENGTH;
            await netmcStream.Lock.WaitAsync(token).ConfigureAwait(false);
            await netmcStream.WriteVarIntAsync(fullSize, token).ConfigureAwait(false);
            await netmcStream.WriteVarIntAsync(0, token).ConfigureAwait(false);
            packetStream.Position = 0;
            await packetStream.CopyToAsync(netmcStream, token).ConfigureAwait(false);
            netmcStream.Lock.Release();
        }


        #endregion
        #region Sync
        public void SendPacket(MemoryStream packet, int id)
        {

            ThrowIfDisposed();
            netmcStream.Lock.Wait();

            if (_compressionThreshold > 0)
            {
                Span<byte> idData = stackalloc byte[5];

                int idLen = id.GetVarIntLength(idData);


                int fullSize = idLen + (int)packet.Length;

                if (fullSize >= _compressionThreshold)
                {
                    using (MemoryStream compressedPacket = new MemoryStream())
                    using (ZLibStream zlibStream = new ZLibStream(compressedPacket, CompressionMode.Compress))
                    {
                        packet.Position = 0;
                        zlibStream.Write(idData.Slice(0, idLen));
                        packet.CopyTo(zlibStream);



                        int comperssPacketLen = (int)compressedPacket.Length + fullSize.GetVarIntLength();
                        netmcStream.WriteVarInt(comperssPacketLen);
                        netmcStream.WriteVarInt(fullSize);

                        compressedPacket.Position = 0;
                        compressedPacket.CopyTo(netmcStream);

                    }
                }
                else
                {
                    fullSize++;

                    netmcStream.WriteVarInt(fullSize);
                    netmcStream.Write(ZERO_VARINT);
                    netmcStream.Write(idData.Slice(0, idLen));
                    packet.Position = 0;
                    packet.CopyTo(netmcStream);

                   
                }
            }
            else
            {
                SendPacketWithoutCompression(packet, id);
            }
            netmcStream.Flush();
            netmcStream.Lock.Release();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SendPacketWithoutCompression(MemoryStream packet, int id)
        {
            ThrowIfDisposed();
            // packet.Write(writer);
            int Packetlength = (int)packet.Length;

            byte[] idData = new byte[5];
            int len = id.GetVarIntLength(idData);
            //Записываем длину всего пакета
            netmcStream.WriteVarInt(Packetlength + len);
            //Записываем ID пакета
            netmcStream.Write(idData, 0, len);

            packet.Position = 0;
            //Все данные пакета перекидваем в интернет
            packet.CopyTo(netmcStream);



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
                    ZLibStream zlibStream = new ZLibStream(dataStream, CompressionMode.Decompress);
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
            if (_disposedStream)
            {
                if (netmcStream != null)
                {
                    netmcStream.Close();
                    netmcStream.Dispose();
                    netmcStream = null;
                }
            }

            GC.SuppressFinalize(this);
        }


    }


}
