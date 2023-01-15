﻿using McProtoNet.Core.IO;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.IO.Compression;

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

            int len = netmcStream.ReadVarInt();
            if (_compressionThreshold <= 0)
            {

                (int id, int id_len) = await netmcStream.ReadVarIntAndLenAsync(token);
                len -= id_len;
                MemoryStream pack = new MemoryStream();

                byte[] buffer = new byte[len];
                int read;
                while (len > 0)
                {
                    read = await netmcStream.ReadAsync(buffer, 0, len, token);
                    len -= read;
                    pack.Write(buffer, 0, read);
                }
                pack.Position = 0;

                return (id, pack);
            }

            int sizeUncompressed = await netmcStream.ReadVarIntAsync(token);
            if (sizeUncompressed > 0)
            {
                len -= sizeUncompressed.GetVarIntLength();

                using (MemoryStream dataStream = new MemoryStream())
                {
                    byte[] r_data = new byte[len];
                    do
                    {
                        int read = await netmcStream.ReadAsync(r_data.AsMemory(), token);
                        len -= read;
                        dataStream.Write(r_data, 0, read);
                    } while (len > 0);
                    dataStream.Position = 0;

                    using (ZLibStream zlibStream = new ZLibStream(dataStream, CompressionMode.Decompress))
                    {

                        int id = zlibStream.ReadVarInt();

                        sizeUncompressed -= id.GetVarIntLength();

                        byte[] uncompressdata = new byte[sizeUncompressed];
                        zlibStream.Read(uncompressdata, 0, sizeUncompressed);
                        zlibStream.Close();
                        zlibStream.Dispose();
                        var packet = new MemoryStream(uncompressdata);

                        return (id, packet);
                    }
                }

            }

            {

                int id = await netmcStream.ReadVarIntAsync(token);
                len -= id.GetVarIntLength() + 1;
                MemoryStream dataStream = new MemoryStream();

                byte[] buffer = new byte[len];
                int read;
                while (len > 0)
                {
                    read = await netmcStream.ReadAsync(buffer, 0, len, token);
                    len -= read;
                    dataStream.Write(buffer, 0, read);
                }
                dataStream.Position = 0;
                return (id, dataStream);
            }

        }
        public async Task SendPacketAsync(MemoryStream packet, int id, CancellationToken token = default)
        {

            ThrowIfDisposed();
            await netmcStream.Lock.WaitAsync(token).ConfigureAwait(false);

            if (_compressionThreshold > 0)
            {


                byte[] idData = new byte[5];

                int idLen = id.GetVarIntLength(idData);


                int uncompressedSize = idLen + (int)packet.Length;
                if (uncompressedSize >= _compressionThreshold)
                {

                    using (var compressedPacket = new MemoryStream())
                    using (var zlibStream = new ZLibStream(compressedPacket, CompressionMode.Compress))
                    {
                        await zlibStream.WriteAsync(idData.AsMemory(0, idLen), token).ConfigureAwait(false);

                        await packet.CopyToAsync(zlibStream, token).ConfigureAwait(false);
                        await zlibStream.FlushAsync(token);//<--- Bug Fix

                        int uncompressedSizeLength = uncompressedSize.GetVarIntLength();

                        int fullSize = uncompressedSizeLength + (int)compressedPacket.Length;



                        await netmcStream.WriteVarIntAsync(fullSize, token).ConfigureAwait(false);

                        await netmcStream.WriteVarIntAsync(uncompressedSize, token).ConfigureAwait(false);

                        compressedPacket.Position = 0;
                        await compressedPacket.CopyToAsync(netmcStream, token).ConfigureAwait(false);
                    }

                }
                else
                {
                    uncompressedSize++;

                    await netmcStream.WriteVarIntAsync(uncompressedSize, token).ConfigureAwait(false);
                    await netmcStream.WriteAsync(ZERO_VARINT, token).ConfigureAwait(false);
                    await netmcStream.WriteAsync(idData.AsMemory(0, idLen), token).ConfigureAwait(false);

                    await packet.CopyToAsync(netmcStream).ConfigureAwait(false);


                }
            }
            else
            {
                await SendPacketWithoutCompressionAsync(packet, id, token).ConfigureAwait(false);
            }
            await netmcStream.FlushAsync(token);
            netmcStream.Lock.Release();
        }

        private async Task SendPacketWithoutCompressionAsync(MemoryStream packet, int id, CancellationToken token)
        {
            ThrowIfDisposed();
            // packet.Write(writer);
            int Packetlength = (int)packet.Length;

            byte[] idData = new byte[5];
            int len = id.GetVarIntLength(idData);
            //Записываем длину всего пакета
            await netmcStream.WriteVarIntAsync(Packetlength + len, token);
            //Записываем ID пакета
            await netmcStream.WriteAsync(idData, 0, len, token);


            //Все данные пакета перекидваем в интернет
            await packet.CopyToAsync(netmcStream, token);


        }


        #endregion
        #region Sync
        #region Send


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void SendPacket(MemoryStream packet, int id)
        {

            ThrowIfDisposed();
            netmcStream.Lock.Wait();

            if (_compressionThreshold > 0)
            {
                Span<byte> idData = stackalloc byte[5];

                int idLen = id.GetVarIntLength(idData);

                //Длина ID+DATA
                int uncompressedSize = idLen + (int)packet.Length;

                if (uncompressedSize >= _compressionThreshold)
                {

                    using (var compressedPacket = new MemoryStream())
                    {
                        using (var zlibStream = new ZLibStream(compressedPacket, CompressionMode.Compress, true))
                        {
                            zlibStream.WriteVarInt(id);
                            packet.CopyTo(zlibStream);
                        }
                        int uncompressedSizeLength = uncompressedSize.GetVarIntLength();

                        int fullSize = uncompressedSizeLength + (int)compressedPacket.Length;

                        netmcStream.WriteVarInt(fullSize);

                        netmcStream.WriteVarInt(uncompressedSize);

                        compressedPacket.Position = 0;
                        compressedPacket.CopyTo(netmcStream);




                    }

                }
                else
                {
                    #region Short                    
                    uncompressedSize++;

                    netmcStream.WriteVarInt(uncompressedSize);

                    netmcStream.Write(ZERO_VARINT);

                    netmcStream.Write(idData.Slice(0, idLen));

                    packet.CopyTo(netmcStream);
                    #endregion
                }
            }
            else
            {
                SendPacketWithoutCompression(packet, id);
            }
            netmcStream.Flush();
            netmcStream.Lock.Release();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private void SendPacketWithoutCompression(MemoryStream packet, int id)
        {

            ThrowIfDisposed();
            // packet.Write(writer);
            int Packetlength = (int)packet.Length;


            Packetlength += id.GetVarIntLength();

            //Записываем длину всего пакета           
            netmcStream.WriteVarInt(Packetlength);
            //Записываем ID пакета
            netmcStream.WriteVarInt(id);

            //Все данные пакета перекидваем в интернет
            packet.CopyTo(netmcStream);


        }
        #endregion
        static Random rand = new();
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public (int, MemoryStream) ReadNextPacket()
        {
            #region New

            ThrowIfDisposed();
            int len = netmcStream.ReadVarInt();

            if (_compressionThreshold <= 0)
            {

                int id = netmcStream.ReadVarInt();
                len -= id.GetVarIntLength();


                byte[] buffer = new byte[len];

                netmcStream.ReadToEnd(buffer, len);

                return (id, new MemoryStream(buffer));
            }

            int sizeUncompressed = netmcStream.ReadVarInt();


            if (sizeUncompressed != 0)
            {

                len -= sizeUncompressed.GetVarIntLength();

                byte[] net_data = new byte[len];
                netmcStream.ReadToEnd(net_data, len);

                using (MemoryStream ms = new MemoryStream(net_data))
                using (ZLibStream zlibStream = new ZLibStream(ms, CompressionMode.Decompress))
                {

                    int id = zlibStream.ReadVarInt();

                    sizeUncompressed -= id.GetVarIntLength();


                    byte[] unc_data = new byte[sizeUncompressed];
                    zlibStream.ReadToEnd(unc_data, sizeUncompressed);



                    var result = new MemoryStream(unc_data);
                    result.Position = 0;
                    return (id, result);
                }

            }
            else
            {

                #region Short
                int id = netmcStream.ReadVarInt();
                len -= (id.GetVarIntLength() + 1);
                byte[] buffer = new byte[len];
                netmcStream.ReadToEnd(buffer, len);
                return (id, new MemoryStream(buffer));
                #endregion
            }

            #endregion
            //int len = netmcStream.ReadVarInt();

            //MemoryStream dataStream = new MemoryStream();

            //byte[] buffer = new byte[len];
            //int read;
            //while (len > 0)
            //{
            //    read = netmcStream.Read(buffer, 0, len);
            //    len -= read;
            //    dataStream.Write(buffer, 0, read);
            //}
            //dataStream.Position = 0;

            //if (_compressionThreshold > 0)
            //{

            //    int sizeUncompressed = dataStream.ReadVarInt();
            //    if (sizeUncompressed != 0)
            //    {

            //        ZLibStream zlibStream = new ZLibStream(dataStream, CompressionMode.Decompress);
            //        byte[] uncompressdata = new byte[sizeUncompressed];
            //        zlibStream.Read(uncompressdata, 0, sizeUncompressed);
            //        zlibStream.Close();
            //        zlibStream.Dispose();
            //        dataStream = new MemoryStream(uncompressdata);
            //    }

            //}

            //int id = dataStream.ReadVarInt();

            //return (id, dataStream);
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
