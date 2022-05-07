using Ionic.Zlib;
using McProtoNet.API.IO;
using System.Net.Sockets;

namespace McProtoNet.API.Networking
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
            this.netmcStream = new NetworkMinecraftStream(networkStream);
        }

        public PacketReaderWriter(Socket socket) : this(new NetworkStream(socket))
        {

        }
        public PacketReaderWriter(TcpClient tcpClient) : this(tcpClient.GetStream())
        {
            
        }

        public void Dispose()
        {
            this.netmcStream.Dispose();
            netmcStream = null;
        }


        public async Task<(int, MinecraftStream)> ReadNextPacketAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                int len = await netmcStream.ReadVarIntAsync(token);
                token.ThrowIfCancellationRequested();
                // Console.WriteLine("len " + len);
                byte[] receivedata = new byte[len];
                await netmcStream.ReadAsync(receivedata.AsMemory(0, len), token);


                var mcs = new MinecraftStream(receivedata);
                if (_compressionThreshold > 0)
                {

                    int sizeUncompressed = mcs.ReadVarInt();
                    if (sizeUncompressed != 0)
                    {
                        ZlibStream zlibStream = new ZlibStream(mcs.BaseStream, CompressionMode.Decompress);
                        byte[] uncompressdata = new byte[sizeUncompressed];
                        zlibStream.Read(uncompressdata);
                        zlibStream.Close();
                        mcs.BaseStream = new MemoryStream(uncompressdata);
                    }

                }
                int id = mcs.ReadVarInt();
                return (id, mcs);
            }
            catch
            {
                throw;
            }
        }


        public async Task SendPacketAsync(IPacket packet, int id, CancellationToken token = default)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(packet, nameof(packet));
                if (_compressionThreshold > 0)
                {
                    using (MinecraftStream packetStream = new MinecraftStream())
                    {
                        packetStream.WriteVarInt(id);
                        packet.Write(packetStream);

                        int to_Packetlength = (int)packetStream.Length;

                        if (to_Packetlength >= _compressionThreshold)
                        {
                            await SendLongPacketAsync(packetStream, to_Packetlength, token);
                        }
                        else
                        {
                            await SendShortPacketAsync(packetStream, token);
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
        private async Task SendPacketWithoutCompressionAsync(IPacket packet, int id, CancellationToken token)
        {
            using (MinecraftStream packetStream = new MinecraftStream())
            {
                packet.Write(packetStream);
                int Packetlength = (int)packetStream.Length;

                await netmcStream.Lock.WaitAsync(token);
                await netmcStream.WriteVarIntAsync(id.GetVarIntLength() + Packetlength, token);
                await netmcStream.WriteVarIntAsync(id, token);
                packetStream.Position = 0;
                packetStream.CopyTo(netmcStream);
                netmcStream.Lock.Release();
            }
        }

        private async Task SendLongPacketAsync(MinecraftStream packetStream, int to_Packetlength, CancellationToken token)
        {
            using (MemoryStream memstream = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(memstream, CompressionMode.Compress))
                {
                    packetStream.CopyTo(stream);
                }
                packetStream.BaseStream = memstream;
            }
            int fullSize = (int)packetStream.Length + to_Packetlength.GetVarIntLength();

            await netmcStream.Lock.WaitAsync(token);

            await netmcStream.WriteVarIntAsync(fullSize, token);
            await netmcStream.WriteVarIntAsync(to_Packetlength, token);
            packetStream.Position = 0;
            packetStream.CopyTo(netmcStream);

            netmcStream.Lock.Release();
        }



        private async Task SendShortPacketAsync(MinecraftStream packetStream, CancellationToken token)
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
            this._compressionThreshold = threshold;
        }
    }


}
