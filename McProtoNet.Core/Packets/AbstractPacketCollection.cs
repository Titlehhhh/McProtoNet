namespace McProtoNet.Core.Packets
{
    public abstract class AbstractPacketCollection : IPacketCollection
    {
        public abstract int TargetProtocolVersion { get; }

        public readonly Dictionary<PacketCategory, Dictionary<int, Type>> ClientPackets = new();
        public readonly Dictionary<PacketCategory, Dictionary<int, Type>> ServerPackets = new();

        public abstract Dictionary<PacketCategory, IPacketProvider> GetAllPackets(PacketSide side);

        public abstract Dictionary<int, Type> GetClientPacketsByCategory(PacketCategory category);

        public abstract Dictionary<int, Type> GetServerPacketsByCategory(PacketCategory category);

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~AbstractPacketCollection()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
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

            _disposed = true;
        }
        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("PacketCollection" + TargetProtocolVersion);
        }
    }
}
