namespace McProtoNet.API.Packets
{
    public class PacketRepository : IPacketRepository
    {
        private Dictionary<PacketCategory, IPacketProvider> AllPAckets;

        public PacketRepository(Dictionary<PacketCategory, IPacketProvider> allPAckets)
        {
            AllPAckets = allPAckets;
        }

        public void Dispose()
        {
            AllPAckets = null;
            GC.SuppressFinalize(this);
        }

        public IPacketProvider GetPackets(PacketCategory category)
        {
            return AllPAckets[category];
        }
    }
}
