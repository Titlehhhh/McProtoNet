namespace McProtoNet.API.Packets
{
    public interface IPacketProviderServer
    {
        Dictionary<int, Type> GetPacketsByName(string name);
    }
}
