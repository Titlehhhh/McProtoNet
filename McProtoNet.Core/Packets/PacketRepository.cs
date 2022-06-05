namespace McProtoNet.Core.Packets
{
    public class PacketRepository : IPacketRepository
    {
        public Dictionary<PacketCategory, IPacketProvider> AllPAckets { get; set; }

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
