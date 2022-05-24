namespace McProtoNet.API.Packets
{
    public interface IPacketProviderClient
    {
        Dictionary<int, Type> GetPacketsByName(string name);
    }
}
