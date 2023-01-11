namespace McProtoNet.Core.Packets
{
    public interface IPacketCollection
    { 
            
    }


    public class PacketCollection
    {
        public readonly Dictionary<PacketCategory, Dictionary<int, Type>> ClientPackets = new();
        public readonly Dictionary<PacketCategory, Dictionary<int, Type>> ServerPackets = new();


        public PacketCollection()
        {
            ClientPackets.Add(PacketCategory.Status, new()
            {
                {0x00, typeof(StatusQueryPacket) },
                {0x01, typeof(StatusPingPacket) }
            });
            ServerPackets.Add(PacketCategory.Status, new()
            {
                {0x00, typeof(StatusResponsePacket) },
                {0x01, typeof(StatusPongPacket) }
            });
            ClientPackets.Add(PacketCategory.HandShake, new()
            {
                {0, typeof(HandShakePacket) }
            });
            ServerPackets.Add(PacketCategory.HandShake, new());

        }
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~PacketCollection()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;
            
                foreach (var v in ClientPackets)
                {
                    v.Value.Clear();
                }
                ClientPackets.Clear();

                foreach (var v in ServerPackets)
                {
                    v.Value.Clear();
                }
                ServerPackets.Clear();
            

            
        }
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("PacketCollection");
        }
    }
}
