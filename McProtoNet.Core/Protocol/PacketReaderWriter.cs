using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;

namespace McProtoNet.Core.Protocol
{
    public sealed class PacketReaderWriter<TPack> : IPacketReaderWriter<TPack> where TPack : IProtocol, new()
    {
        public IProtocol Protocol { get; } = new TPack();

        public IPacketRepository PacketRepository { get; }

        private IMinecraftProtocol minecraftProtocol;
        public PacketReaderWriter(PacketSide side, IMinecraftProtocol minecraftProtocol)
        {
            PacketRepository = new PacketRepository(Protocol.PacketCollection.GetAllPackets(side));
            this.minecraftProtocol = minecraftProtocol;
        }
        private MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader();
        public MinecraftPacket<TPack> ReadNextPacket(PacketCategory category)
        {
            (int id, MemoryStream data) = minecraftProtocol.ReadNextPacket();
            if (PacketRepository.GetPackets(category).TryGetInputPacket(id, out MinecraftPacket<TPack> packet))
            {
                reader.BaseStream = data;
                packet.Read(reader);
                return packet;
            }
            else
            {
                data.Dispose();
                throw new InvalidOperationException($"Input Packet {id} notFound");
            }
        }

        public void SendPacket(MinecraftPacket<TPack> packet, PacketCategory category)
        {
            bool ok = this.PacketRepository.GetPackets(category).TryGetOutputId(packet, out int id);
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

        public void Dispose()
        {

        }

    }
}

