namespace McProtoNet.API.Packets
{
    public interface IPacketRepository : IDisposable
    {
        IPacketProvider GetPackets(PacketCategory category);
    }
}
