namespace McProtoNet.Core.Packets
{

    public interface IPacketCollection : IDisposable
    {
        Dictionary<PacketCategory, IPacketProvider> GetAllPackets(PacketSide side);

        int TargetProtocolVersion { get; }


        Dictionary<int, Type> GetClientPacketsByCategory(PacketCategory category);
        Dictionary<int, Type> GetServerPacketsByCategory(PacketCategory category);


    }
}
