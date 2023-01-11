using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using System.Net.Sockets;

namespace McProtoNet.Core.Protocol
{
    public sealed class PacketReaderWriter : IPacketReaderWriter
    {

        private IMinecraftProtocol minecraftProtocol;
        private IPacketProvider packets;

        public PacketReaderWriter(IMinecraftProtocol minecraftProtocol, IPacketProvider packets)
        {
            this.minecraftProtocol = minecraftProtocol;
            this.packets = packets;
        }

        private MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader();
        public MinecraftPacket ReadNextPacket()
        {
            (int id, MemoryStream data) = minecraftProtocol.ReadNextPacket();
            if (packets.TryGetInputPacket(id, out IInputPacket packet))
            {
                reader.BaseStream = data;
                packet.Read(reader);
                return (MinecraftPacket)packet;
            }
            else
            {
                data.Dispose();
                throw new InvalidOperationException($"Input Packet {id} notFound");
            }
        }

        public void SendPacket(MinecraftPacket packet)
        {
            bool ok = packets.TryGetOutputId(packet, out int id);
            if (ok)
            {
                using (MemoryStream data = new MemoryStream())
                {
                    IMinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter(data);
                    packet.Write(writer);
                    minecraftProtocol.SendPacket(data, id);

                }
            }
            else
            {
                throw new InvalidOperationException($"Output Packet {id} notFound");
            }
        }
        ~PacketReaderWriter()
        {
            Dispose();
        }
        private bool disposed = false;

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            minecraftProtocol?.Dispose();
            minecraftProtocol = null;
            packets?.Dispose();
            packets = null;
            GC.SuppressFinalize(this);
        }

        public void SwitchEncryption(byte[] privateKey)
        {
            minecraftProtocol.SwitchEncryption(privateKey);
        }

        public void SwitchCompression(int threshold)
        {
            minecraftProtocol.SwitchCompression(threshold);
        }
    }
}

