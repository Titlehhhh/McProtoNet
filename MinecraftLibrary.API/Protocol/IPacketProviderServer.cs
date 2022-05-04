namespace ProtoLib.API.Protocol
{
    public interface IPacketProviderServer
    {
        Dictionary<int, Type> StatusPackets { get; }
        Dictionary<int, Type> LoginPackets { get; }
        Dictionary<int, Type> GamePackets { get; }
    }
}
