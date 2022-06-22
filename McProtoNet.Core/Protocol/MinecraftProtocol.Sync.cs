using Ionic.Zlib;
using McProtoNet.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet.Core.Protocol
{
    public sealed partial class MinecraftProtocol : IPacketProtocol
    {
        public void SendPacket(Packet packet, int id)
        {
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
            int len = netmcStream.ReadVarInt();

            MemoryStream dataStream = new MemoryStream();
            byte[] buffer = new byte[len];
            int read = 0;
            while(len > 0)
            {
                read = netmcStream.Read(buffer, 0, len);
                len -= read;
                dataStream.Write(buffer,0, read);
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
    }
}
