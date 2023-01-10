using McProtoNet.Core.Packets;

namespace McProtoNet.Core.Protocol
{
    public interface IPacketReaderWriter<TPack> : IDisposable where TPack : IProtocol
    {
        public IProtocol Protocol { get; }
        public IPacketRepository PacketRepository { get; }
       
        public MinecraftPacket<TPack> ReadNextPacket(PacketCategory category);
        public void SendPacket(MinecraftPacket<TPack> packet, PacketCategory category);
    }
}

