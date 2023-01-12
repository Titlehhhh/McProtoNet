1using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using System.Net.Sockets;

namespace McProtoNet.Core.Protocol
{
    public sealed class PacketReaderWriter : IPacketReaderWriter
    {

        private IMinecraftProtocol minecraftProtocol;
        private IPacketProvider packets;
        private bool _disposeProtocol;

        public PacketReaderWriter(IMinecraftProtocol minecraftProtocol, IPacketProvider packets, bool disposeProtocol)
        {
            this.minecraftProtocol = minecraftProtocol;
            this.packets = packets;
            _disposeProtocol = disposeProtocol;
        }

        private MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader();

        public async Task<MinecraftPacket> ReadNextPacketAsync(CancellationToken cancellationToken = default)
        {
            (int id, MemoryStream data) = await minecraftProtocol.ReadNextPacketAsync(cancellationToken);
            using (data)
            {
                if (packets.TryGetInputPacket(id, out IInputPacket packet))
                {
                    reader.BaseStream = data;
                    packet.Read(reader);
                    return (MinecraftPacket)packet;
                }
            }
            throw new InvalidOperationException($"Input Packet {id} notFound");
        }


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
        public async Task SendPacketAsync(MinecraftPacket packet, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            bool ok = packets.TryGetOutputId(packet, out int id);

            if (ok)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IMinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter(ms);
                    packet.Write(writer);
                    await minecraftProtocol.SendPacketAsync(ms, id, cancellationToken);
                }
            }
            else
            {
                throw new InvalidOperationException($"Output Packet {id} notFound");
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
            if (_disposeProtocol)
            {
                minecraftProtocol?.Dispose();
                minecraftProtocol = null;
            }
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

