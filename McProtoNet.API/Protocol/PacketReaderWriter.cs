
using Ionic.Zlib;
using McProtoNet.API.IO;
using System.Diagnostics;
using System.Net.Sockets;

namespace McProtoNet.API.Protocol
{
    public sealed class PacketReaderWriter : IPacketReaderWriter
    {
        private const int ZERO_VARLENGTH = 1;//default(int).GetVarIntLength();
        private NetworkMinecraftStream netmcStream;

        private int _compressionThreshold;



        public PacketReaderWriter(NetworkMinecraftStream netmcStream)
        {
            this.netmcStream = netmcStream;
        }
        public PacketReaderWriter(NetworkStream networkStream)
        {
            netmcStream = new NetworkMinecraftStream(networkStream);
        }

        public PacketReaderWriter(Socket socket) : this(new NetworkStream(socket))
        {

        }
        public PacketReaderWriter(TcpClient tcpClient) : this(tcpClient.GetStream())
        {

        }

        public void Dispose()
        {
            netmcStream.Dispose();
            netmcStream = null;
        }


        public async Task<(int, MemoryStream)> ReadNextPacketAsync(CancellationToken token)
        {
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


        public async Task SendPacketAsync(Packet packet, int id, CancellationToken token = default)
        {
            Trace.WriteLine("1: " + packet.GetType().Name);
            if (id == 0x01)
            {

            }
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
            Trace.WriteLine("2: " + packet.GetType().Name);

        }
        private async Task SendPacketWithoutCompressionAsync(Packet packet, int id, CancellationToken token)
        {
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
            netmcStream.SwitchEncryption(privateKey);
        }

        public void SwitchCompression(int threshold)
        {
            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold));
            _compressionThreshold = threshold;
        }
    }


}
