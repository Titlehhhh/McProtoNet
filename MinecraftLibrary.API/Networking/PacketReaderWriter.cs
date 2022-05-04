using Ionic.Zlib;
using ProtoLib.API.IO;

namespace ProtoLib.API.Networking
{
    public sealed class PacketReaderWriter : IPacketReaderWriter
    {
        private const int ZERO_VARLENGTH = 1;//default(int).GetVarIntLength();
        private readonly NetworkMinecraftStream netmcStream;




        public PacketReaderWriter(NetworkMinecraftStream netstream)
        {
            this.netmcStream = netstream;
        }

        public int CompressionThreshold { get; set; }

        public NetworkMinecraftStream NetStream => netmcStream;

        public void Dispose()
        {
            NetStream.Dispose();
        }


        public async Task<(int, MinecraftStream)> ReadNextPacketAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                int len = await NetStream.ReadVarIntAsync(token);
                token.ThrowIfCancellationRequested();
                // Console.WriteLine("len " + len);
                byte[] receivedata = new byte[len];
                await NetStream.ReadAsync(receivedata.AsMemory(0, len), token);


                var mcs = new MinecraftStream(receivedata);
                if (CompressionThreshold > 0)
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


        public async Task WritePacketAsync(IPacket packet, int id, CancellationToken token = default)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(packet, nameof(packet));
                if (CompressionThreshold > 0)
                {
                    using (MinecraftStream packetStream = new MinecraftStream())
                    {
                        packetStream.WriteVarInt(id);
                        packet.Write(packetStream);

                        int to_Packetlength = (int)packetStream.Length;

                        if (to_Packetlength >= CompressionThreshold)
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

                await NetStream.Lock.WaitAsync(token);
                await NetStream.WriteVarIntAsync(id.GetVarIntLength() + Packetlength, token);
                await NetStream.WriteVarIntAsync(id, token);
                packetStream.Position = 0;
                packetStream.CopyTo(NetStream);
                NetStream.Lock.Release();
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

            await NetStream.Lock.WaitAsync(token);

            await NetStream.WriteVarIntAsync(fullSize, token);
            await NetStream.WriteVarIntAsync(to_Packetlength, token);
            packetStream.Position = 0;
            packetStream.CopyTo(NetStream);

            NetStream.Lock.Release();
        }



        private async Task SendShortPacketAsync(MinecraftStream packetStream, CancellationToken token)
        {
            int fullSize = (int)packetStream.Length + ZERO_VARLENGTH;
            await NetStream.Lock.WaitAsync(token);
            await NetStream.WriteVarIntAsync(fullSize, token);
            await NetStream.WriteVarIntAsync(0, token);
            packetStream.Position = 0;
            packetStream.CopyTo(NetStream);
            NetStream.Lock.Release();
        }
    }


}
