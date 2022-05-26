namespace McProtoNet.API.Packets
{
    public interface IPacketCollection
    {
        int TargetProtocolVersion { get; }

        Dictionary<int, Type> GetClientPacketsByCategory(PacketCategory category);
        Dictionary<int, Type> GetServerPacketsByCategory(PacketCategory category);
    }
}
